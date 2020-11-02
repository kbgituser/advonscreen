using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dal.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }        
        //[Display(Name = "Почта")]
        //public string Email { get; set; }        
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Display(Name = "Блокированный")]
        public bool Blocked { get; set; }
    }
}
