using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dal.Models
{
    public class PaginatedListItems<T>
    {
        public IQueryable<T> Items { get; set; }
        public int TotalRecords { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}
