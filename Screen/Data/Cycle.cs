using Dal.Data;
using Dal.Data.Migrations;
using Dal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;


namespace Screen.Data
{
    public class Cycle
    {
        private ApplicationDbContext _context;

        public Cycle(ApplicationDbContext Context)
        {
            _context = Context;
        }
        public int PointId { get; set; }
        private string advText;
        public Point Point { get; set; }
        public Advertisement CurrentAdvertisement { get; set; }
        public Advertisement NextAdvertisement { get; set; }
        public int AdvertisementDuration { get; set; }

        private void StartShow()
        {
            if (Point == null)
                Point = GetPoint();

            var spareAd = new Advertisement();
            spareAd.PointId = Point.Id;
            spareAd.Point = Point;
            spareAd.Text = "<div class='text-center'>На данный момент объявлений нет</div>";
            spareAd.Duration = 10;
            spareAd.FontSize = Point.RecommendedFontSize;

            while (Point.TurnedOn)
            {
                SetNextAdvertisement(spareAd);
                if (CurrentAdvertisement != null)
                {
                    Task.Delay(CurrentAdvertisement.Duration * 1000).Wait();
                }
                

                // подумать как обновлять состояние
                Point = GetPoint();
            }
            if (!Point.TurnedOn)
            {
                advText = "Показ объявлений по техническим причинам приостановлен";                
                Task.Delay(30000).Wait();
            }
            StartShow();
        }
        public Point GetPoint()
        {
            if (PointId != 0)
            {
                var resultPoint = _context.Points.Where(p => p.Id == PointId).AsNoTracking().FirstOrDefault();
                if (resultPoint == null)
                    return _context.Points.AsNoTracking().FirstOrDefault();
            }
            return _context.Points.AsNoTracking().FirstOrDefault();
        }

        private void SetNextAdvertisement(Advertisement spareAd)
        {
            if (NextAdvertisement != null)
            {
                CurrentAdvertisement = NextAdvertisement;
                NextAdvertisement = GetNext();
            }
            else
            {
                CurrentAdvertisement = GetNext();
                NextAdvertisement = GetNext();
            }
            if (CurrentAdvertisement == null && spareAd != null) CurrentAdvertisement = spareAd;

            advText = (CurrentAdvertisement != null) ? CurrentAdvertisement.Text : "";
        }


        public Advertisement GetNext()
        {
            Advertisement result;
            if (CurrentAdvertisement == null)
            {
                result = GetFirstAdvertisement();
                return result;
            }
            var advs = _context.Advertisements.Where(a => a.Point == Point && a.AdvertisementStatus.Name == "Active" && a.StartDate > CurrentAdvertisement.StartDate).OrderBy(a => a.StartDate).AsNoTracking();
            //advs = advs.Where(elem => elem.CreateDate > CurrentAdvertisement.CreateDate).OrderBy(a=>a.CreateDate).AsNoTracking();
            if (advs.Any())
            {
                result = advs.Where(elem => elem.StartDate > CurrentAdvertisement.StartDate).Include(a => a.Point).AsNoTracking().FirstOrDefault(); // faster than try-catch
            }
            else
            {
                result = GetFirstAdvertisement();
            }
            return result;
        }

        public Advertisement GetFirstAdvertisement()
        {
            return _context.Advertisements.Where(a => a.Point == Point && a.AdvertisementStatus.Name == "Active").OrderBy(a => a.StartDate).Include(a => a.Point).AsNoTracking().FirstOrDefault();
        }
    }
}
