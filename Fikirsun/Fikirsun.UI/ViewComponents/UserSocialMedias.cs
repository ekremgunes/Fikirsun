using Fikirsun.DAL.Context;
using Fikirsun.Entities;
using Fikirsun.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fikirsun.UI.ViewComponents
{
    public class UserSocialMedias : ViewComponent
    {
        private readonly FikirsunContext _db;
        public UserSocialMedias(FikirsunContext context)
        {
            _db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? name)
        {
            var user = new AppUser();
            var model = new UserSocialMediaModel();

            if (string.IsNullOrEmpty(name))
            {
                 user = _db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            }
            else {
                 user = _db.Users.FirstOrDefault(x => x.UserName == name);
            }
            if (user != null)
            {
                model.ExtraLink = user.ExtraLink;
                model.SocialMedia = user.SocialMedia; 
            }          

            return View(model);
        }
    }
}
