using System;
using System.Collections.Generic;
using System.Text;


namespace Starter.Models
{
    public class ApplicationUser
    {
        
        public string Name { get; set; }
        //[Display(Name = "Имя")]
        //public string FirstName { get; set; }
        //[Display(Name = "Фамилия")]
        //public string LastName { get; set; }
        //[Display(Name = "Отчество")]
        //public string SurName { get; set; }        
        public DateTime CreateDate { get; set; }
        public bool Blocked { get; set; }
        public virtual ICollection<Advertisement> Advertisements { get; set; }
    }
}
