using Fikirsun.DAL.Context;
using Fikirsun.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fikirsun.UI.ViewComponents
{
    public class PopularPosts : ViewComponent
    {
        private readonly FikirsunContext _db;

        public PopularPosts(FikirsunContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = await _db.Posts.Include(x => x.comments).ThenInclude(x => x.replies).ToListAsync();

            var popularPosts = posts.OrderByDescending(post => Popularity.Invoke(post, Popularity.Priority.Like)).Take(5).ToList();

            return View(popularPosts);
        }


    }


}
