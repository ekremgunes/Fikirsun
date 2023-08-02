namespace Fikirsun.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public short categoryOrder { get; set; }
        public List<Post> posts { get; set; }
    }
}
