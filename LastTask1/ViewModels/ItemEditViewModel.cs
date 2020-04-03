using LastTask1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.ViewModels
{
    public class ItemEditViewModel
    {
        public User UserNow { get; set; }

        public Item Item { get; set; }

        public Dictionary<string, string> Fields { get; set; }
    }
}
