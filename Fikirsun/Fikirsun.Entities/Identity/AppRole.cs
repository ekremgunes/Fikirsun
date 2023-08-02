using Microsoft.AspNetCore.Identity;

namespace Fikirsun.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public virtual ICollection<AppUserRole> UserRoles { get; set; }

    }
}
