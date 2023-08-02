namespace Fikirsun.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime signDate { get; set; }
        public byte NotifyType { get; set; }
        public bool isRead { get; set; }
        public string? returnUrl { get; set; }
        public int userId { get; set; }
        public AppUser user { get; set; }
    }
}
