using LastTask1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<Collection> Collections { get; set; }
        public List<Item> Items { get; set;}

    } 
}
