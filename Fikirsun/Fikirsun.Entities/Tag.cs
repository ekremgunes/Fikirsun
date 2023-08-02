namespace Fikirsun.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int postId { get; set; }
        public Post post { get; set; }
    }
}
