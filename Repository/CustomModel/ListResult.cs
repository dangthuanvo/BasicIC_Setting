using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.CustomModel
{
    public class ListResult<T>
    {
        public List<T> items { get; set; }
        public long total { get; set; }

        public ListResult()
        {
            items = new List<T>();
            total = 0;
        }
        public ListResult(List<T> items, long totalItems)
        {
            this.items = items;
            this.total = totalItems;
        }
    }
}
