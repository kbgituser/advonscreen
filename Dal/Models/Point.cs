using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace Dal.Models
{
    public class Point
    {
        public int Id { get; set; }
        [Display(Name = "Наименование точки")]
        public string Name { get; set; }
        [Display(Name = "Адрес")]
        public string Address { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Дата ввода")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Высота")]
        public int Height { get; set; }
        [Display(Name = "Ширина")]
        public int Width { get; set; }
        [Display(Name = "Рекомендуемый размер шрифта")]
        public int RecommendedFontSize { get; set; }
        public bool TurnedOn { get; set; }

    }
}
