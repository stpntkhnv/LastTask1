using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.Models
{
    public class Item
    {
        public string Type { get; set; }
     
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CollectionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Tags { get; set; } 
        public string ImageUrl { get; set; }
        public string CollectionName { get; set; }
        public string OptionalFields { get; set; }
        public int nLikes { get; set; }
        public int nComments { get; set; }

    }
}
