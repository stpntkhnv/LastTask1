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
        public MarkdownSharp.Markdown markdown = new MarkdownSharp.Markdown();
        private readonly CommentContext _commentContext;
        private readonly LikeContext _likeContext;

        public CollectionController(LikeContext likeContext, CommentContext commentContext, CollectionContext context, ItemContext itemContext, UserManager<User> userManager)
        {
            _itemContext = itemContext;
            _collectionContext = context;
            _userManager = userManager;
            _commentContext = commentContext;
            _likeContext = likeContext;
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
            List<Collection> AllCollections = _collectionContext.Collections.ToList().OrderBy(i => i.nItems).ToList();
            
            
            MyCollectionsViewModel model = new MyCollectionsViewModel
            {
                User = User,
                Collections = AllCollections
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

            string description = markdown.Transform(Description);
            string title = markdown.Transform(Title);
            Collection collection = new Collection
            { 
                Id = Guid.NewGuid().ToString(), 
                UserName = User.UserName, 
                UserId = User.Id,
                nItems = 0,
                Title = title,
                Theme = Theme,
                Description = description,
                ShortDescription = ShortDescription,
                Fields = JsonSerializer.Serialize(dictionary),
                Img = ImageUrl,
                Type = "Collection",
                Date = DateTime.Now
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
            var comments = _commentContext.Comments.ToList();
            var likes = _likeContext.Likes.ToList();
            foreach (var item in items)
            {
                foreach(var comment in comments)
                {
                    if(comment.ItemId == item.Id)
                    {
                        user.nComments--;
                        _commentContext.Remove(comment);
                    }
                }
                foreach(var like in likes)
                {
                    if(like.ItemId == item.Id)
                    {
                        user.nLikes--;
                        _likeContext.Remove(like);
                    }
                }
                if (item.CollectionId == collectionId)
                {
                    _itemContext.Remove(item);
                    user.nItems--;
                }
            }
            user.nCollections--;
            await _userManager.UpdateAsync(user);
            await _likeContext.SaveChangesAsync();
            await _commentContext.SaveChangesAsync();
            await _itemContext.SaveChangesAsync();
            return RedirectToAction("Index", "Profile", new { userName = userName });
        }

    }
}