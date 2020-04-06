using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LastTask1.Models;
using LastTask1.ViewModels;

namespace LastTask1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ItemContext _itemContext;
        private readonly CollectionContext _collectionContext;
        private readonly TagContext _tagContext;

        

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, TagContext tagContext, ItemContext itemContext, CollectionContext collectionContext)
        {
            _logger = logger;
            _collectionContext = collectionContext;
            _itemContext = itemContext;
            _tagContext = tagContext;
        }

        public IActionResult Index() 
        {
            List<Collection> AllCollections = _collectionContext.Collections.ToList().OrderByDescending(i => i.nItems).ToList();
            List<Collection> Collections = new List<Collection>();
            if (AllCollections.ToArray().Length > 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    Collections.Add(AllCollections[i]);
                }
            }
            else Collections = AllCollections;
            
            List<Item> AllItems = _itemContext.Items.ToList().OrderByDescending(i => i.Date).ToList();
            List<Item> Items = new List<Item>();
            if (AllItems.ToArray().Length > 20)
            {
                for (int i = 0; i < 20; i++)
                {
                    Items.Add(AllItems[i]);
                }
            }
            else Items = AllItems;
            
            
            HomeViewModel model = new HomeViewModel
            {
                Items = Items,
                Collections = Collections,
                Tags = _tagContext.Tags.ToList()
            };
            return View(model); 
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
