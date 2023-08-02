namespace Fikirsun.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public uint likeCount { get; set; }
        public DateTime createdDate { get; set; }
        public bool isAnswer { get; set; }
        public bool isBanned { get; set; }

        public int? userId { get; set; }
        public AppUser? user { get; set; }
        public int postId { get; set; }
        public Post Post { get; set; }
        public List<Reply> replies { get; set; }
    }
}
