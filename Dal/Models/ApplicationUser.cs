using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Dal.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Имя")]
        public string Name { get; set; }
        //[Display(Name = "Имя")]
        //public string FirstName { get; set; }
        //[Display(Name = "Фамилия")]
        //public string LastName { get; set; }
        //[Display(Name = "Отчество")]
        //public string SurName { get; set; }
        
        [Display(Name = "Дата создания")]
        public DateTime CreateDate { get; set; }
        public bool Blocked { get; set; }
    }
}
