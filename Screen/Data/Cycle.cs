using Dal.Data;
using Dal.Data.Migrations;
using Dal.Models;
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
        public Point Point { get; set; }
        public Advertisement CurrentAdvertisement { get; set; }
        public Advertisement NextAdvertisement { get; set; }
        public System.Threading.Timer aTimer;
        public int AdvertisementDuration { get; set; }

        public void Start()
        {
            aTimer = new System.Threading.Timer(SetNextAdvertisement, null, 0, CurrentAdvertisement.Duration*1000);
        }
        private void SetNextAdvertisement(object state)
        {
            CurrentAdvertisement = GetNext();
            NextAdvertisement = GetNext();
            if (aTimer == null)
                aTimer = new System.Threading.Timer(SetNextAdvertisement, null, 0, CurrentAdvertisement.Duration * 1000);
            ;
            aTimer.Change(0, CurrentAdvertisement.Duration*1000);
            
        }
        public Advertisement GetNext()
        {
            Advertisement result;
            if (CurrentAdvertisement == null)
            {
                result = _context.Advertisements.Where(a => a.Point == Point).OrderBy(a => a.CreateDate).FirstOrDefault();
                return result;
            }
            var advs = _context.Advertisements.Where(a => a.Point == Point).OrderBy(a => a.CreateDate);
            result =  advs.Where(elem => elem.CreateDate < CurrentAdvertisement.CreateDate).FirstOrDefault(); // faster than try-catch
            return result;
        }
    }
}
