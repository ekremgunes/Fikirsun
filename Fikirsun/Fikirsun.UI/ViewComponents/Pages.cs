using Fikirsun.DAL.Context;
using Microsoft.AspNetCore.Mvc;

namespace Fikirsun.UI.ViewComponents
{
    public class Pages : ViewComponent
    {
        private readonly FikirsunContext _db;

        public Pages(FikirsunContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var pages = _db.Pages.ToList();
            return View(pages);
        }
    }
}
