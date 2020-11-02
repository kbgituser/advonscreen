using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dal.Models
{
    public class PointPrice
    {
        public int Id { get; set; }
        public int PointId { get; set; }
        [Display(Name = "Точка")]
        public virtual Point Point { get; set; }
        [Display(Name = "Количество секунд")]
        public int Seconds { get; set; }
        [Display(Name = "Цена")]
        public float Price { get; set; }
        [Display(Name = "Дата")]
        public DateTime SetDate { get; set; }
    }
}
