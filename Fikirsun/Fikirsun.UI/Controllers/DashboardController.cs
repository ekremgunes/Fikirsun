using Fikirsun.DAL.Context;
using Fikirsun.Entities;
using Fikirsun.Tools;
using Fikirsun.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fikirsun.UI.Controllers
{
    /* Diğer kullanıcıları , ayarları ,kategoriler, ve diğer yöneticileri yönetme belki */
    [Authorize(Roles = "Admin,Moderator,Manager")]
    public class DashboardController : Controller
    {
        private readonly FikirsunContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;


        public DashboardController(FikirsunContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Index()
        {
            ViewBag.PostCount = _db.Posts.Count();
            ViewBag.MemberCount = _db.Users.Count();
            ViewBag.CommentCount = _db.Comments.Count();
            ViewBag.ReplyCount = _db.Replies.Count();

            return View();
        }
        #region siteSettings
        [Authorize(Roles = "Admin")]
        public IActionResult Settings()
        {
            var settings = _db.Settings.First();
            var model = new SettingsUpdateModel
            {
                Email = settings.Email,
                ExtraLink = settings.ExtraLink,
                Facebook = settings.Facebook,
                FacebookVisibility = settings.FacebookVisibility,
                Instagram = settings.Instagram,
                InstagramVisibilty = settings.InstagramVisibility,
                SiteDescription = settings.SiteDescription,
                Twitter = settings.Twitter,
                SiteLogo = settings.SiteLogo,
                TwitterVisibility = settings.TwitterVisibility,
                Youtube = settings.Youtube,
                YoutubeVisibility = settings.YoutubeVisibility

            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Settings(SettingsUpdateModel model, IFormFile? logoImg)
        {
            if (ModelState.IsValid)
            {
                var settings = _db.Settings.FirstOrDefault();
                settings!.ExtraLink = model.ExtraLink;
                settings.Facebook = model.Facebook;
                settings.Twitter = model.Twitter;
                settings.Youtube = model.Youtube;
                settings.Email = model.Email;
                settings.Instagram = model.Instagram;
                settings.SiteDescription = model.SiteDescription;

                settings.TwitterVisibility = model.TwitterVisibility;
                settings.FacebookVisibility = model.FacebookVisibility;
                settings.InstagramVisibility = model.InstagramVisibilty;
                settings.YoutubeVisibility = model.YoutubeVisibility;
                if (logoImg != null)
                {
                    var newlogoPath = await FileHelper.ReplaceFile(settings.SiteLogo, logoImg);
                    settings.SiteLogo = newlogoPath;

                }
                _db.SaveChanges();
                TempData["alerts"] = Alert.ViewAlert(AlertType.Success, "Site ayarlarınız başarıyla güncellendi");
                return RedirectToAction("Settings");
            }
            TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, "Site ayarlarınız güncellenemedi");
            return View(model);
        }

        [Authorize(Roles = "Admin,Manager")]

        public IActionResult SeoSettings()
        {
            var settings = _db.Settings.FirstOrDefault();
            if (settings is null)
            {
                TempData["alerts"] = Alert.ViewAlert(AlertType.Error, "Site ayarlarına ulaşılamadı");
                return Redirect("/Dashboard/Index");

            }
            ViewBag.seo = settings.SiteSeo;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public JsonResult SeoSettings(string seo)
        {
            if (string.IsNullOrEmpty(seo))
            {
                return Json(false);
            }

            var settings = _db.Settings.FirstOrDefault();

            if (settings == null)
            {
                return Json(false);
            }
            settings!.SiteSeo = seo;
            _db.SaveChanges();
            TempData["alerts"] = Alert.ViewAlert(AlertType.Success, "Seo başarıyla güncellendi");

            return Json(true);
        }

        #endregion

        #region Kullanıcı Yönetimi (admin) -
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Users()
        {
            var users = new List<AppUser>();
            if (User.IsInRole("Admin"))
            {
                users = await _db.Users
                            .Include(x => x.UserRoles)
                            .ThenInclude(x => x.Role)
                            .Where(x => x.UserRoles.FirstOrDefault()!.Role.Name != "Admin")
                            .ToListAsync();
                return View(users);

            }
            else if (User.IsInRole("Manager"))
            {
                users = await _db.Users
                            .Include(x => x.UserRoles)
                            .ThenInclude(x => x.Role)
                            .Where(x =>
                               x.UserRoles.FirstOrDefault()!.Role.Name != "Admin" &&
                               x.UserRoles.FirstOrDefault()!.Role.Name != "Manager"
                               )
                            .ToListAsync();
                return View(users);

            }
            return View(users);
        }
        [Authorize(Roles = "Admin,Manager")]

        public IActionResult UpdateUser(int id)
        {
            var user = _db.Users
                        .Include(x => x.UserRoles)
                        .ThenInclude(ur => ur.Role)
                        .FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                if (User.IsInRole("Admin"))
                {
                    ViewBag.Roles = _db.Roles.Where(x => x.Name != Roles.Admin).ToList();

                }
                else
                {
                    if (user.UserRoles.FirstOrDefault()!.Role.Name == Roles.Manager || user.UserRoles.FirstOrDefault()!.Role.Name == Roles.Admin)
                    {
                        TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"Bu kullanıcıya erişim yetkiniz yok");
                        return RedirectToAction("Users");
                    }
                    ViewBag.Roles = _db.Roles.Where(x => x.Name != Roles.Admin && x.Name != Roles.Manager).ToList();
                }

                ViewBag.User = user;
                ViewBag.userRole = user.UserRoles.FirstOrDefault()!.Role;
                var model = new UpdateUserRoleModel()
                {
                    roleId = user.UserRoles.FirstOrDefault()!.Role.Id,
                    userId = user.Id
                };
                return View(model);
            }
            TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"Kullanıcı bulunamadı , id:{id}");
            return RedirectToAction("Users");
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUserRoleModel model)
        {
            var isManager = User.IsInRole("Manager");
            var user = _db.Users
                .Include(x => x.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault(x => x.Id == model.userId);
            if (user == null)
            {
                TempData["alerts"] = Alert.ViewAlert(AlertType.Error, "Kullanıcı bulunamadı");
                return RedirectToAction("Users");
            }
            var userRoleId = user.UserRoles.FirstOrDefault()!.RoleId;
            if (ModelState.IsValid)
            {
                if (model.roleId != userRoleId)
                {
                    var newRole = await _roleManager.FindByIdAsync(model.roleId.ToString());
                    var oldRole = await _roleManager.FindByIdAsync(userRoleId.ToString());
                    if (newRole.Name == Roles.Admin)
                    {
                        TempData["alerts"] = Alert.ViewAlert(AlertType.Error, "Bu rolü atama yetkiniz yok !");
                        return RedirectToAction("Users");
                    }
                    else if (newRole is null || oldRole is null)
                    {
                        TempData["alerts"] = Alert.ViewAlert(AlertType.Error, "Kullanıcı veya rol bilgisine ulaşılamadı !");
                        return RedirectToAction("Users");

                    }
                    if (isManager)
                    {
                        if (newRole.Name == Roles.Manager)
                        {
                            TempData["alerts"] = Alert.ViewAlert(AlertType.Error, "Bu rolü atama yetkiniz yok !");
                            return RedirectToAction("Users");
                        }

                    }
                    var removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole.Name);
                    if (removeResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, newRole.Name);
                        //bildirim gönder
                        if (newRole.Name != Roles.Member)
                        {
                            _db.Notifications.Add(new Notification
                            {
                                Content = $"Merhaba {user.UserName} sana bir rol atadık :) Yeni rolün      '{newRole.Name}'.Yeniden giriş yaparak rolüne erişebilirsin.",
                                returnUrl = "/Account/Logout",
                                userId = user.Id,
                                NotifyType = (byte)NotificationType.Info,
                                signDate = DateTime.Now
                            }); ;
                            _db.SaveChanges();
                        }

                        TempData["alerts"] = Alert.ViewAlert(AlertType.Success, $"{user.UserName} kullanıcısının rolü '{newRole.Name}' olarak değiştirildi");
                        return RedirectToAction("Users");

                    }
                    TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, "Kullanıcı Rolü değiştirilirken bir hata meydana geldi");
                }
                else
                {
                    TempData["alerts"] = Alert.ViewAlert(AlertType.Info, "Kullanıcı rolünde bir değişiklik yapılmadı");
                }

                return RedirectToAction("Users");
            }
            if (isManager)
            {
                ViewBag.Roles = _db.Roles.Where(x => x.Name != Roles.Admin && x.Name != Roles.Manager).ToList();

            }
            else
            {
                ViewBag.Roles = _db.Roles.Where(x => x.Name != Roles.Admin).ToList();

            }
            ViewBag.User = user;
            ViewBag.userRole = user.UserRoles.FirstOrDefault()!.Role;

            TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, "Kullanıcı Rolü değiştirilemedi");
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DeleteUser(int id)
        {
            return Json(true);
        }

        #endregion

        #region SpamKelimeler +
        [Authorize(Roles = "Admin,Manager,Moderator")]

        public IActionResult Spams()
        {
            var spams = _db.SpamWords.ToList();
            return View(spams);
        }
        [Authorize(Roles = "Admin,Manager,Moderator")]

        public IActionResult CreateSpam()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Moderator")]

        public async Task<IActionResult> CreateSpam(CreateSpamModel model)
        {
            if (ModelState.IsValid)
            {
                var spams = _db.SpamWords.AsQueryable().ToList();
                if (spams.FirstOrDefault(x => x.Name.ToLower() == model.Name.ToLower()) != null)
                {
                    TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"{model.Name}, Spam kelimelerde Mevcut");
                    return View(model);
                }
                _db.SpamWords.Add(new SpamWord()
                {
                    Name = model.Name
                });
                await _db.SaveChangesAsync();
                TempData["alerts"] = Alert.ViewAlert(AlertType.Success, $"{model.Name} Spamlara Eklendi");

                return RedirectToAction("Spams");
            }
            TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"{model.Name} Spamlara Eklenemedi");
            return View(model);
        }
        [Authorize(Roles = "Admin,Manager,Moderator")]

        public IActionResult UpdateSpam(int id)
        {
            var entity = _db.SpamWords.FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"Güncellenecek spam kelime bulunamadı");

                return RedirectToAction("Spams");
            }
            var model = new UpdateSpamModel
            {
                Id = id,
                Name = entity.Name
            };
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Moderator")]

        public async Task<IActionResult> UpdateSpam(UpdateSpamModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _db.SpamWords.FirstOrDefault(x => x.Id == model.Id);
                if (entity == null)
                {
                    TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"Güncellenecek spam kelime bulunamadı");

                    return RedirectToAction("Spams");
                }

                entity.Name = model.Name;

                await _db.SaveChangesAsync();

                TempData["alerts"] = Alert.ViewAlert(AlertType.Success, $"{model.Name} Düzenlendi");
                return RedirectToAction("Spams");
            }
            return RedirectToAction("Spams");
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Moderator")]

        public async Task<JsonResult> DeleteSpam(int id)
        {
            var spam = await _db.SpamWords.FindAsync(id);
            if (spam != null)
            {
                _db.SpamWords.Remove(spam);
                await _db.SaveChangesAsync();
                return Json(true);
            }
            return Json("Spam kelime bulunamadı");
        }
        #endregion

        #region Kategori İşlemleri +
        [Authorize(Roles = "Admin,Manager")]

        public IActionResult Categories()
        {
            var categories = _db.Categories.Include(x => x.posts).ToList();
            return View(categories);
        }
        [Authorize(Roles = "Admin,Manager")]

        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> CreateCategory(CategoryCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var categories = _db.Categories.AsQueryable().ToList();
                if (categories.FirstOrDefault(x => x.Name.ToLower() == model.Name.ToLower()) != null)
                {
                    TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"{model.Name}, Kategorilerde Mevcut");
                    return View(model);
                }
                _db.Categories.Add(new Category()
                {
                    Name = model.Name,
                    categoryOrder = model.CategoryOrder
                });
                await _db.SaveChangesAsync();
                TempData["alerts"] = Alert.ViewAlert(AlertType.Success, $"{model.Name} Kategorilere Eklendi");

                return RedirectToAction("Categories");
            }
            TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"{model.Name} Kategorilere Eklenemedi");
            return View(model);
        }
        [Authorize(Roles = "Admin,Manager")]

        public IActionResult UpdateCategory(int id)
        {
            var entity = _db.Categories.FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"Güncellenecek kategori bulunamadı");

                return RedirectToAction("Categories");
            }
            var model = new CategoryUpdateModel
            {
                Id = id,
                CategoryOrder = entity.categoryOrder,
                Name = entity.Name
            };
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> UpdateCategory(CategoryUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _db.Categories.FirstOrDefault(x => x.Id == model.Id);
                if (entity == null)
                {
                    TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"Güncellenecek kategori bulunamadı");

                    return RedirectToAction("Categories");
                }

                entity.Name = model.Name;
                entity.categoryOrder = model.CategoryOrder;

                await _db.SaveChangesAsync();

                TempData["alerts"] = Alert.ViewAlert(AlertType.Success, $"{model.Name} Düzenlendi");
                return RedirectToAction("Categories");
            }
            return RedirectToAction("Categories");
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<JsonResult> DeleteCategory(int id)
        {
            if (_db.Categories.Count() <= 1)
            {
                return Json("Sistemde en az bir kategori olmak zorunda ");
            }
            var category = await _db.Categories.FindAsync(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
                return Json(true);
            }
            return Json("Kategori bulunamadı");
        }
        #endregion

        #region Sayfa işlemleri
        [Authorize(Roles = "Admin")]

        public IActionResult Pages()
        {
            var pages = _db.Pages.ToList();
            return View(pages);
        }
        [Authorize(Roles = "Admin")]

        public IActionResult CreatePage()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreatePage(PageCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var pages = _db.Pages.AsQueryable().ToList();
                if (pages.FirstOrDefault(x => x.Title.ToLower() == model.Title.ToLower()) != null)
                {
                    TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"{model.Title}, Sayfalarda Mevcut");
                    return View(model);
                }
                _db.Pages.Add(new Page()
                {
                    Title = model.Title,
                    Content = model.Content
                });
                await _db.SaveChangesAsync();
                TempData["alerts"] = Alert.ViewAlert(AlertType.Success, $"{model.Title} Sayfalara Eklendi");

                return RedirectToAction("Pages");
            }
            TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"Sayfa Eklenemedi");
            return View(model);
        }
        [Authorize(Roles = "Admin")]

        public IActionResult UpdatePage(int id)
        {
            var entity = _db.Pages.FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"Güncellenecek sayfa bulunamadı");

                return RedirectToAction("Pages");
            }
            var model = new PageUpdateModel
            {
                Id = id,
                Content = entity.Content,
                Title = entity.Title
            };
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdatePage(PageUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _db.Pages.FirstOrDefault(x => x.Id == model.Id);
                if (entity == null)
                {
                    TempData["alerts"] = Alert.ViewAlert(AlertType.Warning, $"Güncellenecek sayfa bulunamadı");

                    return RedirectToAction("Pages");
                }

                entity.Title = model.Title;
                entity.Content = model.Content;

                await _db.SaveChangesAsync();

                TempData["alerts"] = Alert.ViewAlert(AlertType.Success, $"{model.Title} Düzenlendi");
                return RedirectToAction("Pages");
            }
            return RedirectToAction("Pages");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<JsonResult> DeletePage(int id)
        {
            if (_db.Pages.Count() <= 1)
            {
                return Json("Sistemde en az bir sayfa olmak zorunda ");
            }
            var page = await _db.Pages.FindAsync(id);
            if (page != null)
            {

                _db.Pages.Remove(page);
                await _db.SaveChangesAsync();
                return Json(true);
            }
            return Json("Sayfa bulunamadı");
        }
        #endregion

        #region Listeleme İşlemleri
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Posts()
        {
            var posts = _db.Posts.Include(x => x.user).ToList();
            return View(posts);
        }
        [Authorize(Roles = "Admin,Manager,Moderator")]

        public IActionResult Comments()
        {
            var comments = _db.Comments.Include(x => x.user).ToList();
            return View(comments);
        }
        [Authorize(Roles = "Admin,Manager,Moderator")]

        public IActionResult Replies()
        {
            var replies = _db.Replies.Include(x => x.user).ToList();
            return View(replies);
        }
        #endregion
    }
}
