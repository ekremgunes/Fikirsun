namespace Fikirsun.Entities
{
    public class Reply
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime createdDate { get; set; }
        public bool isBanned { get; set; }
        public int? repliedUserId { get; set; }
        public int commentId { get; set; }
        public Comment comment { get; set; }
        public int? userId { get; set; }
        public AppUser? user { get; set; }
    }
}
