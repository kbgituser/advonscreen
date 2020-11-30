
using Newtonsoft.Json;
using Starter;
using Starter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Starter
{
    public class Cycle
    {
        string uriBase;
        HttpClientHandler httpClientHandler = new HttpClientHandler();
        HttpClient client;
        WSClient wsClient;
        public bool InternetConnected;
        public Cycle(int pointId, string uri)
        {
            httpClientHandler.ServerCertificateCustomValidationCallback =
            (message, cert, chain, errors) => { return true; };
            //client = new HttpClient(httpClientHandler);
            wsClient = new WSClient();
            PointId = pointId;
            uriBase = uri;
            //uriBase = "https://adv.kenseler.kz/adsapi/";
        }

        public delegate void AdvChangeHandler();
        public event AdvChangeHandler onAdvChange;

        public void Init()
        {
            
            

            //Task.Run(() => GetPoint());
            GetPoint();
            //GetPoint().GetAwaiter().GetResult();
            //try
            //{
            //    //Point = await GetPoint();
                
            //}
            //catch (Exception exception)
            //{ }
            //finally
            //{ }
        }

        public int PointId { get; set; }
        public string advText;
        public Point Point { get; set; }
        public Advertisement CurrentAdvertisement { get; set; }
        public Advertisement NextAdvertisement { get; set; }
        public int AdvertisementDuration { get; set; }

        public async Task StartShowAsync()
        {
            await Task.Run(() => StartShow());            
            //Task.Factory.StartNew(() => StartShow());
            //Task.Run(StartShow);
            //StartShow();

            //StartShow().GetAwaiter().GetResult();
        }

        public void StartShow()
        {
            if (!InternetConnected)
            {
                return;
            }

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
                if (!InternetConnected)
                {                    
                    return;
                }
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
            if (!InternetConnected) return default(Point);

            Point updatedPoint = wsClient.Get<Point>(uriBase + "getPoint/" + PointId);
            //if (updatedPoint == null)
            //{
            //    Task.Delay(60000).Wait();
            //    GetPoint();
            //}
            return updatedPoint;
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
            onAdvChange();
        }

        public Advertisement GetNext()
        {
            Advertisement result;
            if (CurrentAdvertisement == null)
            {
                //result = GetFirstAdvertisement();
                //return wsClient.Get<Advertisement>(uriBase + "getAdvertisement/" + PointId).Result;
                result = wsClient.Get<Advertisement>(uriBase + "getAdvertisement/" + PointId);            }
            else
            {
                result = wsClient.Get<Advertisement>(uriBase + "getAdvertisement/" + PointId + "/" + CurrentAdvertisement.Id);                
            }
            //result = wsClient.Get<Advertisement>(uriBase + "getAdvertisement/"+PointId+"/"+CurrentAdvertisement.Id).Result;            
            return result;
        }

        //public Advertisement GetFirstAdvertisement()
        //{
        //    return _context.Advertisements.Where(a => a.Point == Point && a.AdvertisementStatus.Name == "Active").OrderBy(a => a.StartDate).Include(a => a.Point).AsNoTracking().FirstOrDefault();
        //}
    }
}
