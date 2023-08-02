namespace Fikirsun.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string postTitle { get; set; }
        public string postContent { get; set; }
        public uint viewCount { get; set; }
        public uint likeCount { get; set; }
        public DateTime createdDate { get; set; }
        public bool isEdited { get; set; }
        public bool isBanned { get; set; }

        public int? userId { get; set; }
        public AppUser? user { get; set; }
        public int categoryId { get; set; }
        public Category category { get; set; }
        public List<Tag> tags { get; set; }
        public List<Comment> comments { get; set; }
    }
}
