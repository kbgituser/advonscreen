using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Models
{
    public class UserPoint
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PointId { get; set; }
        
        public virtual Point Point { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
