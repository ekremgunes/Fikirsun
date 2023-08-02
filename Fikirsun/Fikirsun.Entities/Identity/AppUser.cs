using Microsoft.AspNetCore.Identity;

namespace Fikirsun.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public bool isBanned { get; set; } = false;
        public string? userSubName { get; set; }
        public string? about { get; set; }
        public string profilePhoto { get; set; }
        public bool isDeleted { get; set; } = false;
        public DateTime signDate { get; set; }
        public DateTime? deletedDate { get; set; }
        public string? SocialMedia { get; set; }
        public string? ExtraLink { get; set; }
        public int actionCount { get; set; } = 0;
        public List<Post> posts { get; set; }
        public List<Reply> replies { get; set; }
        public List<Comment> comments { get; set; }
        public List<LikedComment> likedComments { get; set; }
        public List<LikedPost> likedPosts { get; set; }
        public List<Notification> notifications { get; set; }

        public virtual ICollection<AppUserRole> UserRoles { get; set; }


    }
}