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

        

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ItemContext itemContext, CollectionContext collectionContext)
        {
            _logger = logger;
            _collectionContext = collectionContext;
            _itemContext = itemContext;
        }

        public IActionResult Index() 
        {
            HomeViewModel model = new HomeViewModel
            {
                Items = _itemContext.Items.ToList(),
                Collections = _collectionContext.Collections.ToList()
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
