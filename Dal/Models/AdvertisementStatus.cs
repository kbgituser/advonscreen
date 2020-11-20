using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dal.Models
{
    public class AdvertisementStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Статус")]
        public string NameRu { get; set; }
        public virtual ICollection<Advertisement> Advertisements { get; set; }
    }
}
