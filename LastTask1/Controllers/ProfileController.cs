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
        private readonly LikeContext _likeContext;
        private readonly CommentContext _commentContext;

        public ProfileController(CollectionContext context, ItemContext itemContext, UserManager<User> userManager, CommentContext commentContext, LikeContext likeContext)
        {
            _itemContext = itemContext;
            _collectionContext = context;
            _userManager = userManager;
            _commentContext = commentContext;
            _likeContext = likeContext;
        }

        
        // GET: Profile
        public async Task<ActionResult> Index(string userName)
        {
            User User = await _userManager.FindByNameAsync(userName);
            List<Item> AllItems = _itemContext.Items.ToList();
            List<Collection> AllCollections= _collectionContext.Collections.ToList();
            List<Item> Items = new List<Item>();
            List<Collection> Collections = new List<Collection>();
            List<Comment> AllComments = _commentContext.Comments.ToList();
            List<Item> Comments = new List<Item>();
            List<Like> AllLikes = _likeContext.Likes.ToList();
            List<Item> Likes = new List<Item>();
            foreach (var collection in AllCollections)
            {
                if(collection.UserName == User.UserName)
                {
                    Collections.Add(collection);
                }
            };
            foreach(var item in AllItems)
            {
                if(item.UserName == User.UserName)
                {
                    Items.Add(item);
                }
            };
            foreach(var comment in AllComments)
            {
                if(comment.UserName == User.UserName)
                {
                    Item item = _itemContext.Items.Where(o => o.Id == comment.ItemId).SingleOrDefault();
                    Comments.Add(item);
                }
            };
            foreach(var like in AllLikes)
            {
                if(like.UserName == User.UserName)
                {
                    Item item = _itemContext.Items.Where(o => o.Id == like.ItemId).SingleOrDefault();
                    Likes.Add(item);
                }
            }
            ProfileViewModel model = new ProfileViewModel
            {
                Collections = Collections,
                Items = Items,
                User = User,
                Likes = Likes,
                Comments = Comments
            };
            return View(model);
        }
    }
}