using LastTask1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.ViewModels
{
    public class ItemViewModel
    {
        public User User { get; set; }
        public Collection Collection { get; set; }
        public Item Item { get; set; }
        public List<Comment> Comments { get; set; }
        public bool IsLiked { get; set; }
    }
}
