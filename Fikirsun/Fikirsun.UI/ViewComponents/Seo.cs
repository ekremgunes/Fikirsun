using Fikirsun.DAL.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fikirsun.UI.ViewComponents
{
    public class Seo : ViewComponent
    {
        private readonly FikirsunContext _db;
        public Seo(FikirsunContext context)
        {
            _db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var seo = _db.Settings.First()?.SiteSeo;
            ViewBag.seo = seo;
            return View();
        }
    }
}
