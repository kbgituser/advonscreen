using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dal.Models
{
    public class CreateUserViewModel
    {
        [Display(Name = "Почта")]
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        
        [Display(Name = "Имя")]
        public string Name { get; set; }
        
    }
}
