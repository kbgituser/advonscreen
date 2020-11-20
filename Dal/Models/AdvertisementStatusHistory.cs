using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Models
{
    public class AdvertisementStatusHistory
    {
        public int Id { get; set; }
        public int AdvertisementId { get; set; }
        public virtual Advertisement Advertisement { get; set; }
        public int AdvertisementStatusId { get; set; }
        public virtual AdvertisementStatus AdvertisementStatus { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}
