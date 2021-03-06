﻿using System;
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
        private readonly TagContext _tagContext;
        public MarkdownSharp.Markdown markdown = new MarkdownSharp.Markdown();
        
        public ItemController(TagContext tagContext, LikeContext likeContext, ItemContext context, CollectionContext collectionContext, UserManager<User> userManager, CommentContext commentContext)
        {
            _collectionContext = collectionContext;
            _itemContext = context;
            _userManager = userManager;
            _commentContext = commentContext;
            _likeContext = likeContext;
            _tagContext = tagContext;
        }



        public IActionResult Index()
        {
            List<Item> Items = _itemContext.Items.ToList().OrderByDescending(o => o.Date).ToList(); 
            MyItemsViewModel model = new MyItemsViewModel()
            {
                Items = Items
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
                Id = Guid.NewGuid().ToString(),
                Type = "Comment"
            };
            _commentContext.Add(comment);
            User.nComments++;
            await _userManager.UpdateAsync(User);

            await _itemContext.SaveChangesAsync();
            await _commentContext.SaveChangesAsync();
            return RedirectToAction("Item", "Item", new { itemId = itemId});
        }

        public string[] GetTags(string val)
        {
            string[] s = new string[1];
            s[0] = " ";
            if (val == null) return s;
            if(val.Contains(" "))
            {
                s = val.Split(" ");
                val = s.Last();
            };
            
            List<Tag> AllTags = _tagContext.Tags.ToList();
            List<string> SuitableTags = new List<string>();
            foreach(var tag in AllTags)
            {
                if (tag.Text.Contains(val))
                    SuitableTags.Add(tag.Text);

            }
            foreach (var b in s)
            {
                SuitableTags.Remove(b);
            }
                string[] result = SuitableTags.ToArray(); 
            return result;
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
                User.nLikes--;
            }
            else
            {
                Like like = new Like()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userName,
                    ItemId = itemId
                };
                User.nLikes++;
                _likeContext.Add(like);
                Item.nLikes++;

            }
            await _userManager.UpdateAsync(User);

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
                JsonConvert.SerializeObject(Comments)+

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
            //var markdown = new MarkdownSharp.Markdown();
            foreach (var colStr in CollectionFields)
            {
                string type = colStr.Key;
                foreach (var colName in colStr.Value)
                {
                    string name = colName;
                    string value = Request.Form[name];
                    if (value == "" || value == null)
                        value = "off";
                    string s = type + "$$$$$" + markdown.Transform(value); 
                    ItemFields.Add(markdown.Transform(name), s);
                }                                   
            }
            
            string html = markdown.Transform(Description);
            string title = markdown.Transform(Title);
            
            Item item = new Item
            {
                Id = Guid.NewGuid().ToString(),
                CollectionId = collectionId,
                UserName = collection.UserName,
                Title = title,
                Description = html,
                Tags = Tags,
                CollectionName = collection.Title,
                ImageUrl = ImageUrl,
                OptionalFields = JsonConvert.SerializeObject(ItemFields),
                nComments = 0,
                nLikes = 0,
                Type = "Item",
                Date = DateTime.Now
            };
            List<Tag> Tagss = _tagContext.Tags.ToList();
            string[] ListTag = Tags.Split(" ");
            foreach(var tag in ListTag)
            {
                var result = _tagContext.Tags.Where(o => o.Text == tag).SingleOrDefault();
                if (result == null) {
                    Tag Tag = new Tag() 
                    { 
                        Id = Guid.NewGuid().ToString(), 
                        Text = tag
                    };
                    _tagContext.Add(Tag);
                        };
            }
            
            await _tagContext.SaveChangesAsync();
            _itemContext.Add(item);
            collection.nItems++;
            user.nItems++;
            await _userManager.UpdateAsync(user);
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
            User user = await _userManager.FindByNameAsync(item.UserName);

            _itemContext.Items.Remove(item);
            Collection collection = _collectionContext.Collections
                .Where(o => o.Id == item.CollectionId)
                .FirstOrDefault();
            collection.nItems--;
            user.nItems--;
            await _userManager.UpdateAsync(user);

            await _collectionContext.SaveChangesAsync();
            await _itemContext.SaveChangesAsync();
            return RedirectToAction("Index", "Profile", new { userName = collection.UserName });
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
            string html = markdown.Transform(Description);
            string title = markdown.Transform(Title);
            foreach (var colStr in CollectionFields)
            {
                string type = colStr.Key;
                foreach (var colName in colStr.Value)
                {
                    string name = colName;
                    string value = Request.Form[name];
                    if (value == "" || value == null)
                        value = "off";
                    string s = type + "$$$$$" + markdown.Transform(value);
                    ItemFields.Add(markdown.Transform(name), s);
                }
            }

            item.OptionalFields = JsonConvert.SerializeObject(ItemFields);
            item.Title = title;
            item.Description = html;
            item.Tags = Tags;
            item.ImageUrl = ImageUrl;

            await _itemContext.SaveChangesAsync();

            return RedirectToAction("Item", "Item", new { itemId = itemId });
        }

    }
}