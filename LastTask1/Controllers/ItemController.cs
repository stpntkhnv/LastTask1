using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LastTask1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LastTask1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Newtonsoft.Json;

namespace LastTask1.Controllers
{
    public class ItemController : Controller
    {

        private readonly ItemContext _itemContext;
        private readonly CollectionContext _collectionContext;
        private readonly UserManager<User> _userManager;
        private readonly CommentContext _commentContext;
        private readonly LikeContext _likeContext;
        
        public ItemController(LikeContext likeContext, ItemContext context, CollectionContext collectionContext, UserManager<User> userManager, CommentContext commentContext)
        {
            _collectionContext = collectionContext;
            _itemContext = context;
            _userManager = userManager;
            _commentContext = commentContext;
            _likeContext = likeContext;
        }



        public async Task<IActionResult> Index(string itemId)
        {
            Item item = _itemContext.Items
                .Where(o => o.Id == itemId)
                .FirstOrDefault();
            Collection collection = _collectionContext.Collections
                .Where(o => o.Id == item.CollectionId)
                .FirstOrDefault();
            User user = await _userManager.FindByNameAsync(collection.UserName);
            ItemViewModel model = new ItemViewModel()
            {
                Item = item,
                Collection = collection,
                User = user
            };
            return View(model);
        }


        public async Task<IActionResult> CreateComment(string userName, string Text, string itemId)
        {
            User User = await _userManager.FindByNameAsync(userName);
            Item item = _itemContext.Items.Where(o => o.Id == itemId).SingleOrDefault();
            item.nComments++;
            Comment comment = new Comment
            {
                UserName = User.UserName,
                ItemId = itemId,
                Text = Text,
                Id = Guid.NewGuid().ToString()
            };
            _commentContext.Add(comment);
            await _itemContext.SaveChangesAsync();
            await _commentContext.SaveChangesAsync();
            return RedirectToAction("Item", "Item", new { itemId = itemId});
        }


        public async Task<IActionResult> ChangeLike(string userName, string itemId)
        {
            List<Like> Likes = _likeContext.Likes.ToList();
            Item Item = _itemContext.Items.Where(o => o.Id == itemId).SingleOrDefault();
            User User = await _userManager.FindByNameAsync(userName);
            bool IsLiked = false;
            Like Like = new Like();
            foreach(var like in Likes)
            {
                if(like.UserName == User.UserName&&like.ItemId == itemId)
                {
                    IsLiked = true;
                    Like = like;
                }
            }
            if(IsLiked)
            {
                _likeContext.Remove(Like);
                Item.nLikes--;
            }
            else
            {
                Like like = new Like()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userName,
                    ItemId = itemId
                };
                
                _likeContext.Add(like);
                Item.nLikes++;

            }
            await _likeContext.SaveChangesAsync();
            await _itemContext.SaveChangesAsync();
            return RedirectToAction("Item", "Item", new { itemId = itemId });

        }

        public async Task<IActionResult> Item(string itemId)
        {
            List<Like> Likes = _likeContext.Likes.ToList();
            bool IsLiked = false;       
            
            Item item = _itemContext.Items.Where(o => o.Id == itemId).SingleOrDefault();
            User user = await _userManager.FindByNameAsync(item.UserName);
            Collection collection = _collectionContext.Collections.Where(o => o.Id == item.CollectionId).SingleOrDefault();
            List<Comment> Comments = _commentContext.Comments.ToList();
            List<Comment> UserComments = new List<Comment>();
            foreach (var like in Likes)
            {
                if (like.UserName == user.UserName && like.ItemId == itemId)
                {
                    IsLiked = true;                    
                }
            }
            foreach (var comment in Comments)
            {
                if (comment.ItemId == itemId)
                    UserComments.Add(comment);
            }
           
            ItemViewModel model = new ItemViewModel()
            {
                Item = item,
                Collection = collection,
                User = user,
                Comments = UserComments,
                IsLiked = IsLiked
            };
            return View(model);
        }
        public IActionResult AllYourItems()
        {
            HomeViewModel model = new HomeViewModel()
            {
                Items = _itemContext.Items.ToList(),
                Collections = _collectionContext.Collections.ToList()
            };
            return View(model);
        }



        public IActionResult Create(string collectionId)
        {
            Collection collection = _collectionContext.Collections
                .Where(o => o.Id == collectionId)
                .FirstOrDefault();
            ItemCreateViewModel model = new ItemCreateViewModel
            {
                Collection = collection,
                Fields = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(collection.Fields)

            };
            return View(model);
        }

        public JsonResult Search(string val)
        {
            List<Item> Items = _itemContext.Items.Where(o => o.Title.Contains(val) || o.Description.Contains(val) || o.Tags.Contains(val)).ToList();
            List<Comment> Comments = _commentContext.Comments.Where(o => o.Text.Contains(val)).ToList();           
            List<Collection> Collections = _collectionContext.Collections.Where(o => o.Title.Contains(val) || o.Description.Contains(val)).ToList(); 
            string result = "[" + JsonConvert.SerializeObject(Items) + ","+
                JsonConvert.SerializeObject(Collections) + ","+
                JsonConvert.SerializeObject(Comments) +

                "]";
            return Json(result);
        }


     
        public async Task<ActionResult> ItemCreate(string collectionId, string Title, string Description, string ShortDescription, string exitionalString, string Tags, bool goToNew, string ImageUrl)
        {
            Collection collection = _collectionContext.Collections
                .Where(o => o.Id == collectionId)
                .FirstOrDefault();
            User user = await _userManager.FindByNameAsync(collection.UserName);
            Dictionary<string, string[]> CollectionFields = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(collection.Fields);
            Dictionary<string, string> ItemFields = new Dictionary<string, string>();

            foreach (var colStr in CollectionFields)
            {
                string type = colStr.Key;
                foreach (var colName in colStr.Value)
                {
                    string name = colName;
                    string value = Request.Form[name];
                    if (value == "" || value == null)
                        value = "off";
                    string s = type + "$$$$$" + value;
                    ItemFields.Add(name, s);
                }                                   
            }

            Item item = new Item
            {
                Id = Guid.NewGuid().ToString(),
                CollectionId = collectionId,
                UserName = collection.UserName,
                Title = Title,
                Description = Description,
                ShortDescription = ShortDescription,
                Tags = Tags,
                CollectionName = collection.Title,
                ImageUrl = ImageUrl,
                OptionalFields = JsonConvert.SerializeObject(ItemFields)              
            };
            _itemContext.Add(item);
            collection.nItems++;
            user.nItems++;
            await _collectionContext.SaveChangesAsync();
            await _itemContext.SaveChangesAsync();
            if(goToNew == true)
            return RedirectToAction("Create", "Item", new {collectionId = collection.Id});
            return RedirectToAction("Collection", "Collection", new { collectionId = collection.Id });
        }

        public async Task<ActionResult> Delete(string itemId)
        {
            Item item = _itemContext.Items
                .Where(o => o.Id == itemId)
                .FirstOrDefault();
            _itemContext.Items.Remove(item);
            Collection collection = _collectionContext.Collections
                .Where(o => o.Id == item.CollectionId)
                .FirstOrDefault();
            collection.nItems--;
            await _collectionContext.SaveChangesAsync();
            await _itemContext.SaveChangesAsync();
            return RedirectToAction("Index", "Item", new { id = collection.Id });
        }

        public async Task<IActionResult> Edit(string itemId)
        {
            Item item = _itemContext.Items.Where(o => o.Id == itemId).SingleOrDefault();
            User UserNow = await _userManager.FindByNameAsync(item.UserName);
            ItemEditViewModel model = new ItemEditViewModel()
            {
                UserNow = UserNow,
                Item = item,
                Fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.OptionalFields)
            };
            return View(model);
        }

        public async Task<IActionResult> ItemEdit(string itemId, string ImageUrl, string Description, string Tags, string Title)
        {
            Item item = _itemContext.Items.Where(o => o.Id == itemId).SingleOrDefault();
            Collection collection = _collectionContext.Collections
                .Where(o => o.Id == item.CollectionId)
                .FirstOrDefault();
            User user = await _userManager.FindByNameAsync(collection.UserName);
            Dictionary<string, string[]> CollectionFields = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(collection.Fields);
            Dictionary<string, string> ItemFields = new Dictionary<string, string>();

            foreach (var colStr in CollectionFields)
            {
                string type = colStr.Key;
                foreach (var colName in colStr.Value)
                {
                    string name = colName;
                    string value = Request.Form[name];
                    if (value == "" || value == null)
                        value = "off";
                    string s = type + "$$$$$" + value;
                    ItemFields.Add(name, s);
                }
            }

            item.OptionalFields = JsonConvert.SerializeObject(ItemFields);
            item.Title = Title;
            item.Description = Description;
            item.Tags = Tags;
            item.ImageUrl = ImageUrl;

            await _itemContext.SaveChangesAsync();

            return RedirectToAction("Item", "Item", new { itemId = itemId });
        }

    }
}