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
        public Collection Collection { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "ShortDescription")]
        public string ShortDescription { get; set; }

        [Required]
        [Display(Name = "Tags")]
        public string Tags { get; set; }


        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public Item Item { get; set; }

        public Dictionary<string, string> Fields { get; set; }
    }
}
