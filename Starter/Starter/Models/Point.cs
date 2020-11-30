using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Starter.Models
{
    public class Point
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Address { get; set; }
        
        public string Description { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public int Height { get; set; }

        
        public int Width { get; set; }
        
        public int RecommendedFontSize { get; set; }
        
        public float Scale{ get; set; }
        public bool TurnedOn { get; set; }
        
        public int CycleSize { get; set; }

    }
}
