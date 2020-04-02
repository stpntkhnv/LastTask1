using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.ViewModels
{
    public class CollectionCreateViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }

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
        [Display(Name = "Theme")]
        public string Theme { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; }




    }
}
