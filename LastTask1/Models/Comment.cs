using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.Models
{
    public class Comment
    {
        public string Type { get; set; }
       
        public string Id { get; set; }
        public string ItemId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
    }
}
