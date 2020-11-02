using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dal.Models
{
    public class SecondsForAdv
    {
        public int Id { get; set; }
        [Display(Name = "Секунды")]
        public int Seconds { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
    }
}
