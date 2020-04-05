using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.Models
{
    public class Collection
    {
        public string Type { get; set; }
        
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Theme { get; set; }
        public string Img { get; set; }
        public int nItems { get; set; }
        public string Fields { get; set; }
    }
}
