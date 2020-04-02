using LastTask1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.ViewModels
{
    public class CollectionViewModel
    {
        public User User { get; set; }
        public Collection Collection { get; set; }
        public List<Item> Items { get; set; }
        public string[] OptionalFields { get; set; }



    }
}
