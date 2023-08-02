namespace Fikirsun.Entities
{
    public class LikedComment
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public AppUser user { get; set; }
        public int commentId { get; set; }
    }
}
