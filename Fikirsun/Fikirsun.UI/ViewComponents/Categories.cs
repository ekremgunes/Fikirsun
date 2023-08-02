using Fikirsun.DAL.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fikirsun.UI.ViewComponents
{
    public class Categories : ViewComponent
    {
        private readonly FikirsunContext _db;
        public Categories(FikirsunContext context)
        {
            _db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _db.Categories.OrderBy(x => x.categoryOrder).ToListAsync();

            return View(categories);
        }
    }
}
