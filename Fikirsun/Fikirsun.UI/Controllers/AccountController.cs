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
    public class AccountController : Controller
    {
        private readonly FikirsunContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(FikirsunContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _db = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Sign-In-Up
        public IActionResult Login()
        {
            if (User.Identity?.Name != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Login([FromBody] UserLoginModel user)
        {
            var foundUser = await _userManager.FindByNameAsync(user.UserName);

            if (foundUser != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(foundUser, user.Password, true, true);

                if (signInResult.Succeeded)
                {
                    return Json(true);
                }
                return Json("Giriş başarısız,Şifreni kontrol et");

            }
            return Json("Kullanıcı bulunamadı");
        }
        public IActionResult Signup()
        {
            TempData["Page"] = _db.Pages.First()?.Title;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Signup([FromBody] UserSignupModel user)
        {
            TempData["Page"] = _db.Pages.First()?.Title;

            if (ModelState.IsValid)
            {
                var foundUser = await _userManager.FindByNameAsync(user.UserName);
                if (foundUser != null)
                {
                    return Json("Bu kullanıcı adı zaten alınmış ....");
                }

                var foundUserEmail = await _userManager.FindByEmailAsync(user.Email);

                if (foundUserEmail != null)
                {
                    return Json("Bu email adresi başkası tarafından kullanılıyor ....");
                }
                try
                {
                    var newUser = new AppUser { UserName = user.UserName, Email = user.Email };
                    var result = await _userManager.CreateAsync(newUser, user.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newUser, Roles.Member);

                        //bildirim gönder
                        _db.Notifications.Add(new Notification
                        {
                            Content = $"{user.UserName.ToUpperInvariant()} hoşgeldin.Seni aramızda görmekten mutluluk duyuyoruz !",
                            userId = newUser.Id,
                            NotifyType = (byte)NotificationType.Info,
                            signDate = DateTime.Now
                        });
                        _db.SaveChanges();
                        return Json(true);

                    }
                    return Json("Kullanıcı Oluşturulamadı. " + result.Errors.First().Description);
                }
                catch (Exception e)
                {

                    return Json("Beklenmeyen hata. " + e.Message);

                }
            }
            return Json("Kullanıcı Oluşturulamadı ....");
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["alerts"] = Alert.ViewAlert(AlertType.Info, $"Güvenli bir şekilde çıkış yaptınız ");
            return Redirect("/Home/Index");
        }
        #endregion


        #region User        
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {

            var user = await _userManager.FindByNameAsync(User.Identity!.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var usermodel = new UserSettingsModel()
            {
                Email = user.Email,
                About = user.about,
                ExtraLink = user.ExtraLink,
                IsDeleted = user.isDeleted,
                UserName = user.UserName,
                UserSubName = user.userSubName,
                SocialMedia = user.SocialMedia,
                ProfilePhoto = user.profilePhoto

            };
            return View(usermodel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Settings(UserSettingsModel model, IFormFile profilePhotoFile)
        {

            var user = await _userManager.FindByNameAsync(User.Identity!.Name);

            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.NormalizedEmail = model.Email.ToUpper();
            }
            if (user.UserName != model.UserName)
            {
                user.UserName = model.UserName;
                user.NormalizedUserName = model.UserName.ToUpper();
            }
            //kullanıcı resmini  güncelleme 
            if (profilePhotoFile != null)
            {

                if (user.profilePhoto == "defaultPP.png")
                {
                    string updatedPath = await FileHelper.CreateFile(profilePhotoFile);
                    user.profilePhoto = updatedPath;
                }
                else
                {
                    string updatedPath = await FileHelper.ReplaceFile(user.profilePhoto!, profilePhotoFile);
                    user.profilePhoto = updatedPath;
                }
            }
            //kullanıcı bilgilerini güncelleme

            user.SocialMedia = model.SocialMedia;
            user.ExtraLink = model.ExtraLink;
            user.about = model.About;
            user.userSubName = model.UserSubName;


            var updateResult = await _userManager.UpdateAsync(user); // kullanıcı bilgileri veritabanında değişiyor
            if (updateResult.Succeeded)
            {
                await _signInManager.SignOutAsync(); // kullanıcı oturumu kapatlıyor
                await _signInManager.SignInAsync(user, true); // güncel bilgileriyle tekrar oturum açıyor
                return RedirectToAction("Index", "Home");

            }
            return View(model);

        }

        public async Task<IActionResult> UserDetail([FromQuery] string userName)
        {
            if (userName == null)
            {
                return RedirectToAction("Index", "Home"); //returnUrl kullanılabilir
            }

            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return RedirectToAction("Index", "Home"); //returnUrl kullanılabilir
            }
            var userDb = _db.Users
                    .Include(x => x.posts)
                        .ThenInclude(x => x.tags)
                    .Include(x => x.posts)
                        .ThenInclude(x => x.category)
                    .Include(x => x.posts)
                        .ThenInclude(x => x.comments)
                    .FirstOrDefault(x => x.Id == user!.Id);
            var userModel = new UserModel()
            {
                posts = user.posts,
                about = user.about,
                userSubName = user.userSubName,
                Email = user.Email,
                ExtraLink = user.ExtraLink,
                isDeleted = user.isDeleted,
                profilePhoto = user.profilePhoto,
                SocialMedia = user.SocialMedia,
                UserName = user.UserName,
                signDate = user.signDate
            };
            return View(userModel);
        }
        #endregion       

    }
}
