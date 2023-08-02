using Fikirsun.DAL.Context;
using Fikirsun.Entities;
using Fikirsun.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Fikirsun.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly FikirsunContext _db;
        public HomeController(FikirsunContext context)
        {
            _db = context;
        }

        public IActionResult Index([FromQuery] int? category, [FromQuery] string tag, [FromQuery] string search
            )
        {
            int pageSize = 5;

            var posts = new List<Post>();
           
                if (category != null && category > 0 && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(tag))
                {
                    posts = _db.Posts
                     .Include(x => x.user)
                     .Include(x => x.tags)
                     .Include(x => x.comments)
                     .ThenInclude(x => x.replies)
                     .Where(x => x.categoryId == category)
                     .ToList();

                    posts = posts
                        .OrderByDescending(post => Popularity.Invoke(post, Popularity.Priority.Time))
                        .Take(pageSize)
                        .ToList();

                    ViewBag.NoResultMessage = "Bu kategorinin altında herhangi bir soru bulunamadı .";


                }
                else if (!string.IsNullOrEmpty(search))
                {
                    if (category > 0)
                    {
                        posts = _db.Posts
                        .Include(x => x.user)
                        .Include(x => x.tags)
                        .Include(x => x.comments)
                        .ThenInclude(x => x.replies)
                        .Where(x => x.categoryId == category)
                        .Where(x => x.postTitle.Contains(search))
                        .ToList();

                        posts = posts
                            .OrderByDescending(post => Popularity.Invoke(post, Popularity.Priority.Like))
                            .Take(pageSize)
                            .ToList();
                    }
                    else
                    {
                        posts = _db.Posts
                        .Include(x => x.user)
                        .Include(x => x.tags)
                        .Include(x => x.comments)
                        .ThenInclude(x => x.replies)
                        .Where(x => x.postTitle.Contains(search))
                        .ToList();

                        posts = posts
                            .OrderByDescending(post => Popularity.Invoke(post, Popularity.Priority.Like))
                            .Take(pageSize)
                            .ToList();
                    }

                    ViewBag.NoResultMessage = "Aramanızla eşlenen bir soru bulunamadı .";

                }
                else if (!string.IsNullOrEmpty(tag))
                {
                    if (category > 0)
                    {
                        posts = _db.Posts
                          .Include(x => x.user)
                          .Include(x => x.tags)
                          .Include(x => x.comments)
                          .ThenInclude(x => x.replies)
                          .Where(x => x.categoryId == category)
                          .Where(x => x.tags.Any(t => t.Name == tag))
                          .ToList();

                        posts = posts.OrderByDescending(post =>
                            Popularity.Invoke(post, Popularity.Priority.Like
                            ))
                            .Take(pageSize)
                            .ToList();
                    }
                    else
                    {
                        posts = _db.Posts
                          .Include(x => x.user)
                          .Include(x => x.tags)
                          .Include(x => x.comments)
                          .ThenInclude(x => x.replies)
                          .Where(x => x.tags.Any(t => t.Name == tag))
                          .ToList();

                        posts = posts.OrderByDescending(post =>
                            Popularity.Invoke(post, Popularity.Priority.Like
                            ))
                            .Take(pageSize)
                            .ToList();
                    }

                    ViewBag.NoResultMessage = $"'{tag}' etiketini kullanan bir soru bulunamadı .";


                }
                else // en temiz durum KEŞFET
                {
                    posts = _db.Posts
                       .Include(x => x.user)
                       .Include(x => x.tags)
                       .Include(x => x.comments)
                       .ThenInclude(x => x.replies)
                       .ToList();

                    posts = posts
                        .OrderByDescending(post => Popularity.Invoke(post, Popularity.Priority.Time))
                        .Take(pageSize)
                        .ToList();
                    ViewBag.NoResultMessage = "Herhangi bir soru bulunamadı :(";
                }
            
            
           
            return View(posts);

        }

        #region Load More
        public IActionResult LoadMore(
         string? tag, int? category, string? search, int pageIndex = 2)
        {

            int pageSize = 5;
            var posts = new List<Post>();
           
                if (category != null && category > 0 && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(tag))
                {
                    posts = _db.Posts
                     .Include(x => x.user)
                     .Include(x => x.tags)
                     .Include(x => x.category)
                     .Include(x => x.comments)
                     .ThenInclude(x => x.replies)
                     .Where(x => x.categoryId == category)
                     .ToList();

                    posts = posts
                        .OrderByDescending(post => Popularity.Invoke(post, Popularity.Priority.Time))
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    ViewBag.LoadMessage = "Bu kategorinin altında daha fazla soru bulunamadı .";


                }
                else if (!string.IsNullOrEmpty(search))
                {
                    if (category > 0)
                    {
                        posts = _db.Posts
                              .Include(x => x.user)
                              .Include(x => x.tags)
                              .Include(x => x.category)
                              .Include(x => x.comments)
                              .ThenInclude(x => x.replies)
                              .Where(x => x.categoryId == category)
                              .Where(x => x.postTitle.Contains(search))
                              .ToList();
                    }
                    else
                    {
                        posts = _db.Posts
                           .Include(x => x.user)
                           .Include(x => x.tags)
                           .Include(x => x.category)
                           .Include(x => x.comments)
                           .ThenInclude(x => x.replies)
                           .Where(x => x.postTitle.Contains(search))
                           .ToList();
                    }


                    posts = posts
                        .OrderByDescending(post => Popularity.Invoke(post, Popularity.Priority.Like))
                        .Skip(((pageIndex - 1)) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    ViewBag.LoadMessage = "Aramanızla eşlenen daha fazla soru bulunamadı .";

                }
                else if (!string.IsNullOrEmpty(tag))
                {
                    if (category > 0)
                    {
                        posts = _db.Posts
                        .Include(x => x.user)
                        .Include(x => x.category)
                        .Include(x => x.tags)
                        .Include(x => x.comments)
                        .ThenInclude(x => x.replies)
                        .Where(x => x.categoryId == category)
                        .Where(x => x.tags.Any(t => t.Name == tag))
                        .ToList();
                    }
                    else
                    {
                        posts = _db.Posts
                        .Include(x => x.user)
                        .Include(x => x.category)
                        .Include(x => x.tags)
                        .Include(x => x.comments)
                        .ThenInclude(x => x.replies)
                        .Where(x => x.tags.Any(t => t.Name == tag))
                        .ToList();
                    }


                    posts = posts.OrderByDescending(post =>
                        Popularity.Invoke(post, Popularity.Priority.Like
                        ))
                        .ToList();
                    ViewBag.LoadMessage = $"'{tag}' etiketinde daha fazla soru bulunamadı";


                }
                else
                {
                    posts = _db.Posts
                       .Include(x => x.user)
                       .Include(x => x.category)
                       .Include(x => x.tags)
                       .Include(x => x.comments)
                       .ThenInclude(x => x.replies)
                       .ToList();

                    posts = posts
                        .OrderByDescending(post => Popularity.Invoke(post, Popularity.Priority.Time))
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    ViewBag.LoadMessage = "Daha fazla soru bulunamadı :(";
                }
            
           
            return View(posts);

        }
        #endregion

        #region Notification
        [Authorize]
        public IActionResult Notifications() // tarih parametresi alabilir ?
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var notifications = _db.Notifications
                .OrderByDescending(x => x.signDate)
                .Where(x => x.userId == userId)
                .ToList();
            return View(notifications);
        }
        [Authorize]
        [HttpPost]
        public JsonResult SetNotifications()
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var notifications = _db.Notifications
                .OrderBy(x => x.signDate)
                .Where(x => x.userId == userId)
                .ToList();
            foreach (var item in notifications)
            {
                item.isRead = true;
            }
            if (notifications.Any(x => x.signDate < DateTime.Now.AddDays(-7)))
            {
                _db.Notifications.RemoveRange(notifications.Where(x => x.signDate < DateTime.Now.AddDays(-7)).ToList());
            }
            _db.SaveChanges();
            return Json(true);
        }
        #endregion

        #region Page
        public IActionResult Page([FromQuery] string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return RedirectToAction("Index", "Home");
            }
            var page = _db.Pages.FirstOrDefault(x => x.Title == title);
            if (page == null)
            {
                //tempdata
                return RedirectToAction("Index", "Home");
            }

            return View(page);
        }
        #endregion
    }
}