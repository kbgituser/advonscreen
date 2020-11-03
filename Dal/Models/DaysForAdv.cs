using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace Dal.Models
{    
    public class DaysForAdv
    {
        public int Id { get; set; }
        [Display(Name = "Дни")]
        public int Days { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
    }
}
