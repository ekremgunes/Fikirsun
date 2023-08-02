using Fikirsun.Entities;
using Fikirsun.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fikirsun.UI.ViewComponents
{
    public class UserInfo : ViewComponent
    {
        private readonly UserManager<AppUser> _userMananager;

        public UserInfo(UserManager<AppUser> userMananager)
        {
            _userMananager = userMananager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userModel = new UserModel();
            if (User.Identity?.Name != null)
            {
                var user = await _userMananager.FindByNameAsync(User.Identity!.Name);
                var aktifKullanici = await _userMananager.FindByIdAsync(await _userMananager.GetUserIdAsync(user));
                userModel.profilePhoto = aktifKullanici.profilePhoto;
                userModel.UserName = aktifKullanici.UserName;
                userModel.userSubName = aktifKullanici.userSubName;
                userModel.Id = aktifKullanici.Id;
                userModel.about = aktifKullanici.about;
                userModel.isDeleted = aktifKullanici.isDeleted;
            }
            return View(userModel);
        }
    }
}
