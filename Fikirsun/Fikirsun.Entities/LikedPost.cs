namespace Fikirsun.Entities
{
    public class LikedPost
    {
        public int Id { get; set; }
        public int postId { get; set; }
        public int userId { get; set; }
        public AppUser user { get; set; }
    }
}
