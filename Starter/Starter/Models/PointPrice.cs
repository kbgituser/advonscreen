using System;
using System.Collections.Generic;

using System.Text;

namespace Starter.Models
{
    public class PointPrice
    {
        public int Id { get; set; }
        public int PointId { get; set; }

        public virtual Point Point { get; set; }

        public int Seconds { get; set; }

        public float Price { get; set; }

        public DateTime SetDate { get; set; }
    }
}
