using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LastTask1.Models;
using LastTask1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LastTask1.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ItemContext _itemContext;
        private readonly CollectionContext _collectionContext;
        private readonly UserManager<User> _userManager;

        public ProfileController(CollectionContext context, ItemContext itemContext, UserManager<User> userManager)
        {
            _itemContext = itemContext;
            _collectionContext = context;
            _userManager = userManager;
        }

        
        // GET: Profile
        public async Task<ActionResult> Index(string userName)
        {
            User User = await _userManager.FindByNameAsync(userName);
            List<Item> AllItems = _itemContext.Items.ToList();
            List<Collection> AllCollections= _collectionContext.Collections.ToList();
            List<Item> Items = new List<Item>();
            List<Collection> Collections = new List<Collection>();
            foreach(var collection in Collections)
            {
                if(collection.UserName == User.UserName)
                {
                    Collections.Add(collection);
                }
            };
            foreach(var item in Items)
            {
                if(item.UserName == User.UserName)
                {
                    Items.Add(item);
                }
            };
            ProfileViewModel model = new ProfileViewModel
            {
                Collections = Collections,
                Items = Items,
                User = User
            };

            return View(model);
        }
    }
}