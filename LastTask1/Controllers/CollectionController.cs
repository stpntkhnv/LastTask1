using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LastTask1.Models;
using LastTask1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LastTask1.Controllers
{
 
    public class CollectionController : Controller
    {
        private readonly ItemContext _itemContext;
        private readonly CollectionContext _collectionContext;
        private readonly UserManager<User> _userManager;


        
        public CollectionController(CollectionContext context, ItemContext itemContext, UserManager<User> userManager)
        {
            _itemContext = itemContext;
            _collectionContext = context;
            _userManager = userManager;
        }

        public Collection GetCollection(string collectionId)
        {
            Collection collection = _collectionContext.Collections
                            .Where(o => o.Id == collectionId)
                            .FirstOrDefault();
            return collection;
        }

        public async Task<IActionResult> Collection(string collectionId)
        {
            Collection collection = _collectionContext.Collections
                            .Where(o => o.Id == collectionId)
                            .FirstOrDefault();
            
            List < Item > AllItems = _itemContext.Items.ToList();
            List<Item> UserItems = new List<Item>();
            foreach(var item in AllItems)
            {
                if(item.CollectionId == collection.Id)
                {
                    UserItems.Add(item);
                }
            }
            Dictionary<string, string[]> OptFields = JsonSerializer.Deserialize<Dictionary<string, string[]>>(collection.Fields);
            string OptionalStrings = "";
            foreach(var str in OptFields)
            {
                foreach(var val in str.Value)
                {
                    OptionalStrings += val + "break";
                }
            }


            CollectionViewModel model = new CollectionViewModel
            {
                User = await _userManager.FindByNameAsync(collection.UserName),
                Collection = collection,
                Items = UserItems,
                OptionalFields = OptionalStrings.Split("break")
            };
            return View(model);
         }


        

        //GET: Collection
        public async Task<IActionResult> Index(string userName)
        {
            
            User User = await _userManager.FindByNameAsync(userName);
            List<Collection> AllCollections = _collectionContext.Collections.ToList();
            List<Collection> UserCollection = new List<Collection>();
            foreach(var collection in AllCollections)
            {
                if(collection.UserName == User.UserName)
                {
                    UserCollection.Add(collection);
                }
            }

            MyCollectionsViewModel model = new MyCollectionsViewModel
            {
                User = User,
                Collections = UserCollection
            };
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> Create(string userName)
        {
            User User = await _userManager.FindByNameAsync(userName);
            CollectionCreateViewModel model = new CollectionCreateViewModel
            {
                UserName = userName
            };
            return View(model);
        }
        // GET: Collection/Create
        [HttpPost]
        public async Task<ActionResult> CollectionCreate(string userName, string Title, string Theme, string Description, string ShortDescription, string ImageUrl)
        {
            User User = await _userManager.FindByNameAsync(userName);
            string[] integers = Request.Form["Int"];
            string[] strings = Request.Form["Str"];
            string[] dates = Request.Form["Date"];
            string[] booles = Request.Form["Bool"];
            string[] texts = Request.Form["Text"];
            for(int i = 0; i < booles.Length; i++)
            {
                if (booles[i] == "$$$$$")
                    booles[i] = "Incorrect name";
            }
            Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>
            {
                ["int"] = integers,
                ["str"] = strings,
                ["date"] = dates,
                ["bool"] = booles,
                ["text"] = texts
            };

            
            Collection collection = new Collection
            { 
                Id = Guid.NewGuid().ToString(), 
                UserName = User.UserName, 
                UserId = User.Id,
                nItems = 0,
                Title = Title,
                Theme = Theme,
                Description = Description,
                ShortDescription = ShortDescription,
                Fields = JsonSerializer.Serialize(dictionary),
                Img = ImageUrl,
                Type = "Collection"
            };
            User.nCollections++;
            await _userManager.UpdateAsync(User);

            await _collectionContext.Collections.AddAsync(collection);
            await _collectionContext.SaveChangesAsync();
           
            return RedirectToAction("Collection", "Collection", new{userName = userName, collectionId =  collection.Id});
        }

        public async Task<ActionResult> Delete(string collectionId)
        {
            Collection collection = _collectionContext.Collections.Where(o => o.Id == collectionId).SingleOrDefault();
            string userName = collection.UserName;
            User user = await _userManager.FindByNameAsync(userName);
            _collectionContext.Collections.Remove(collection);
            await _collectionContext.SaveChangesAsync();
            var items = _itemContext.Items.ToList();
            foreach (var item in items)
                if (item.CollectionId == collectionId)
                { 
                    _itemContext.Remove(item);
                    user.nItems--;
                }
            user.nCollections--;
            await _userManager.UpdateAsync(user);

            await _itemContext.SaveChangesAsync();
            return RedirectToAction("Index", "Profile", new { userName = userName });
        }

    }
}