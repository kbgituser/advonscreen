using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Dal.Models
{
    public class Advertisement
    {
        public int Id { get; set; }        
        [Display(Name = "Длительность в секундах")]
        public int Duration { get; set; }
        [Display(Name = "Длительность в днях")]
        public int DurationInDays { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Дата начала")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Дата окончания")]
        public DateTime EndDate { get; set; }
        
        [Display(Name = "Заголовок")]
        public string Title { get; set; }
        [Display(Name = "Текст объявления")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        [Display(Name = "Номер объявления")]
        public string AdNumber { get; set; }
        [Display(Name = "Цена")]
        public float Price { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser{ get; set; }
        [Display(Name = "Размер шрифта")]
        public int FontSize { get; set; }

        [Display(Name = "Место объявления")]

        [Required(ErrorMessage = "Нужно выбрать точку")]
        public int PointId { get; set; }
        public virtual Point Point { get; set; }
        public int AdvertisementStatusId { get; set; }
        public virtual AdvertisementStatus AdvertisementStatus { get; set; }

        [Display(Name = "Вид содержания")]
        public AdvertisementType AdvertisementType { get; set; }

        [Display(Name = "Цвет фона")]
        public string BackgroundColor { get; set; } = "#FFFFFF";
        
        [Display(Name = "Фотография")]
        public string ImagePath { get; set; } 
        
        [Display(Name = "Видео")]
        public string Video { get; set; } 
        public virtual ICollection<AdvertisementStatusHistory> AdvertisementStatusHistories { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public AdvertisementStatus GetAdvertisementStatus()
        {
            return AdvertisementStatusHistories.OrderByDescending(a => a.ChangeDate).Any()
                ? AdvertisementStatusHistories.OrderByDescending(a => a.ChangeDate).FirstOrDefault().AdvertisementStatus
                : null
                ;
        }
    }
}
