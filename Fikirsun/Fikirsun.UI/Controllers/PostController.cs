using Fikirsun.DAL.Context;
using Fikirsun.Entities;
using Fikirsun.Tools;
using Fikirsun.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Fikirsun.UI.Controllers
{
    public class PostController : Controller
    {
        private readonly FikirsunContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public PostController(FikirsunContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _db = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index(int id)
        {

            var post = _db.Posts
                .Include(x => x.user)
                .Include(x => x.category)
                .Include(x => x.tags)
                .Include(x => x.comments)
                    .ThenInclude(c => c.replies)
                    .ThenInclude(r => r.user)
                .Include(x => x.comments)
                    .ThenInclude(c => c.user)
                    .FirstOrDefault(x => x.Id == id);

            if (post == null)
            {
                TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, "Post bulunamadı :(");
                return RedirectToAction("Index", "Home");

            }
            var isAnswered = post.comments.FirstOrDefault(x => x.isAnswer == true);

            ViewBag.answer = isAnswered;


            if (User.Identity?.Name != null)
            {
                var user = _db.Users
                    .Include(x => x.likedPosts)
                    .Include(x => x.likedComments)
                    .FirstOrDefault(x => x.UserName == User.Identity!.Name);
                var activeUser = new AppUser
                {
                    profilePhoto = user!.profilePhoto,
                    UserName = user.UserName,
                    Id = user!.Id
                };
                var postComments = post.comments;
                ViewBag.likedComments = user.likedComments
                    .Where(x => x.commentId == (postComments.FirstOrDefault(c => c.Id == x.commentId)?.Id))
                    .ToList();

                ViewBag.ActiveUser = activeUser;

                if (user!.likedPosts.FirstOrDefault(x => x.postId == id) != null)
                {
                    ViewBag.likedBefore = true;
                    return View(post);
                }
                ViewBag.likedBefore = false;
                return View(post);
            }
            ViewBag.likedComments = new List<LikedComment>();
            ViewBag.ActiveUser = new AppUser();
            ViewBag.likedBefore = false;
            return View(post);
        }
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> InsertViewCount([FromBody] int postId)
        {
            var post = _db.Posts.FirstOrDefault(x => x.Id == postId);
            if (post != null)
            {
                post.viewCount += 1;
                await _db.SaveChangesAsync();
                return Json(true);
            }
            return Json(false);
        }
        [Authorize]
        public IActionResult CreatePost()
        {
            ViewBag.Categories = _db.Categories.ToList();
            return View();

        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> CreatePost([FromBody] PostCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var post = new Post()
                {
                    postContent = model.Content,
                    postTitle = model.Title,
                    userId = userId,
                    categoryId = Convert.ToInt32(model.Category)
                };

                _db.Posts.Add(post);
                await _db.SaveChangesAsync();

                if (model.Tags != null)
                {
                    foreach (var tag in model.Tags)
                    {
                        _db.Tags.Add(new Tag() { Name = tag, postId = post.Id });
                    }
                    _db.SaveChanges();
                }

                return Json(true);

            }
            return Json(ModelState.Root.Errors.First().ErrorMessage.ToString());
        }

        [Authorize]
        public IActionResult UpdatePost(int id)
        {
            var post = _db.Posts.Include(x => x.tags).Include(x => x.category).FirstOrDefault(x => x.Id == id);
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (post == null | Convert.ToInt32(userId) != post?.userId)
            {
                RedirectToAction("Index", "Home");
            }

            ViewBag.Categories = _db.Categories.ToList();
            var model = new PostUpdateModel()
            {
                Id = post.Id,
                category = post.category,
                Content = post.postContent,
                Title = post.postTitle,
                Tags = new string[3]
            };
            for (int i = 0; i < post.tags.Count; i++)
            {
                model.Tags[i] = post.tags[i].Name;
            }
            return View(model);

        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> UpdatePost([FromBody] PostUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var post = _db.Posts
                    .Include(x => x.tags)
                    .FirstOrDefault(x => x.Id == model.Id && x.userId == userId);

                if (post == null)
                {
                    return Json("404");
                }
                post.postTitle = model.Title;
                post.categoryId = Convert.ToInt32(model.postCategory);
                post.postContent = model.Content;
                post.isEdited = true;

                var postTags = _db.Tags.Where(x => x.postId == post.Id).ToList(); // ? -O-
                _db.Tags.RemoveRange(postTags); //postun içindeki etiketleri temizle
                _db.SaveChanges();

                if (model.Tags != null) // kullanıcı etiket göndermişsse onları ekle
                {
                    foreach (var tag in model.Tags)
                    {
                        _db.Tags.Add(new Tag() { Name = tag, postId = post.Id });
                    }
                    await _db.SaveChangesAsync();
                }

                return Json(true);
            }
            return Json(ModelState.Root.Errors.First().ErrorMessage.ToString());
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeletePost([FromBody] int postId)
        {

            var post = _db.Posts
                .Include(x => x.user)
                .SingleOrDefault(x => x.Id == postId);
            if (post == null)
            {
                return Json("404");

            }
            if (!User.IsInRole("Admin"))
            {
                if (!User.IsInRole("Manager"))
                {
                    if (User.Identity!.Name != post.user!.UserName)
                    {
                        return Json($"/Post/Index/{postId}");
                    }
                }
            }


            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
            return Json(true);
        }


        [Authorize]
        [HttpPost]
        public JsonResult LikePost([FromBody] int postId)
        {
            var post = _db.Posts.Include(x => x.user).FirstOrDefault(x => x.Id == postId);
            if (post == null)
            {
                return Json("404");
            }
            var user = _db.Users
                .Include(x => x.likedPosts)
                .FirstOrDefault(x => x.UserName == User.Identity!.Name);
            if (user == null)
            {
                return Json(false);
            }
            var isLikedBefore = user!.likedPosts
                                    .FirstOrDefault(x => x.postId == postId);

            if (isLikedBefore != null)
            {
                return Json("Bu soruyu öncesinde beğenmişsiniz");
            }

            var likedPost = new LikedPost()
            {
                postId = postId,
                userId = user!.Id
            };
            _db.LikedPosts.Add(likedPost);
            post.likeCount += 1;
            _db.SaveChanges();

            //bildirim gönder
            if (post.userId != user.Id)
            {
                _db.Notifications.Add(new Notification
                {
                    Content = $"{user.UserName} sorunuzu beğendi !",
                    returnUrl = $"/Post/Index/{post.Id}",
                    userId = post.user!.Id,
                    NotifyType = (byte)NotificationType.Like,
                    signDate = DateTime.Now
                });
                _db.SaveChanges();
            }


            return Json(true);
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> DislikePost([FromBody] int postId)
        {
            var post = _db.Posts.FirstOrDefault(x => x.Id == postId);
            if (post == null)
            {
                return Json("404");
            }
            var user = await _db.Users
                .Include(x => x.likedPosts)
                .FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

            var likedPost = user?.likedPosts.FirstOrDefault(x => x.postId == postId);
            if (likedPost != null)
            {
                _db.LikedPosts.Remove(likedPost);
                post.likeCount -= 1;
                _db.SaveChanges();

            }
            return Json(true);
        }
    }
}
