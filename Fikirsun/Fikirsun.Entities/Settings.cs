namespace Fikirsun.Entities
{
    public class Settings
    {
        public int Id { get; set; }
        public string SiteSeo { get; set; }
        public string SiteDescription { get; set; }
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public string ExtraLink { get; set; }
        public string? Twitter { get; set; }
        public string? Youtube { get; set; }
        public string SiteLogo { get; set; }
        public string Email { get; set; }
        public bool YoutubeVisibility { get; set; }
        public bool TwitterVisibility { get; set; }
        public bool InstagramVisibility { get; set; }
        public bool FacebookVisibility { get; set; }


    }
}
