using System;
using System.Collections.Generic;

using System.Text;

namespace Starter.Models
{
    public class AdvertisementStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string NameRu { get; set; }
        public virtual ICollection<Advertisement> Advertisements { get; set; }
    }
}
