
using Starter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Starter.Models
{
    public class Advertisement
    {
        public int Id { get; set; }        
        
        public int Duration { get; set; }
        
        public int DurationInDays { get; set; }

        
        public DateTime CreateDate { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        
        public string Title { get; set; }
        
        public string Text { get; set; }
        
        public string AdNumber { get; set; }
        
        public float Price { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser{ get; set; }
        
        public int FontSize { get; set; }
        
        public int PointId { get; set; }
        public virtual Point Point { get; set; }
        public int AdvertisementStatusId { get; set; }
        public virtual AdvertisementStatus AdvertisementStatus { get; set; }
        public AdvertisementType AdvertisementType { get; set; }
        public string ImagePath { get; set; }
        public string Video { get; set; }
        public string BackgroundColor { get; set; } = "#FFFFFF";
        public virtual ICollection<AdvertisementStatusHistory> AdvertisementStatusHistories { get; set; }
        public AdvertisementStatus GetAdvertisementStatus()
        {
            return AdvertisementStatusHistories.OrderByDescending(a => a.ChangeDate).Any()
                ? AdvertisementStatusHistories.OrderByDescending(a => a.ChangeDate).FirstOrDefault().AdvertisementStatus
                : null
                ;
        }
    }
}
