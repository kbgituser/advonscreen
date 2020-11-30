using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int AdvertisementId { get; set; }
        public virtual Advertisement Advertisement { get; set; }
        public int Days { get; set; }
        public float Sum { get; set; }
        // Цена есть в классе Advertisement
        public float Price { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
