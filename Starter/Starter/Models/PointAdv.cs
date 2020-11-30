
using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Models
{
    public class PointAdv
    {
        public int Id { get; set; }
        public int PointId { get; set; }
        public virtual Point Point { get; set; }
        public int AdvertisementId { get; set; }
        public virtual Advertisement Advertisement { get; set; }

    }
}
