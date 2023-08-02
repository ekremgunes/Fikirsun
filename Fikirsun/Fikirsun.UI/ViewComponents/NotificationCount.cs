using Fikirsun.DAL.Context;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fikirsun.UI.ViewComponents
{
    public class NotificationCount : ViewComponent
    {
        private readonly FikirsunContext _db;
        public NotificationCount(FikirsunContext context)
        {
            _db = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId < 1)
            {
                return View(0);
            }
            var notifications = _db.Notifications
                .OrderBy(x => x.signDate)
                .Where(x => x.userId == userId && x.isRead == false)
                .Count();
            return View(notifications);
        }
    }
}
