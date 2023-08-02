using Fikirsun.DAL.Context;
using Fikirsun.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fikirsun.UI.ViewComponents
{
    public class SocialMedias : ViewComponent
    {
        private readonly FikirsunContext _db;
        public SocialMedias(FikirsunContext context)
        {
            _db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var settings = _db.Settings.First();
            var model = new SocialMediaModels();

            model.ExtraLink = settings.ExtraLink;
            model.Youtube = settings.Youtube;
            model.YoutubeVisibility = settings.YoutubeVisibility;
            model.Twitter = settings.Twitter;
            model.TwitterVisibility = settings.TwitterVisibility;
            model.Facebook = settings.Facebook;
            model.FacebookVisibility = settings.FacebookVisibility;
            model.Instagram = settings.Instagram;
            model.InstagramVisibilty = settings.InstagramVisibility;

            return View(model);
        }
    }
}
