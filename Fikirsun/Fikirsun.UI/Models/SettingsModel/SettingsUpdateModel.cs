using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class SettingsUpdateModel
    {
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string SiteDescription { get; set; }
        public string? Instagram { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public bool InstagramVisibilty { get; set; }
        public string? Facebook { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public bool FacebookVisibility { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string ExtraLink { get; set; }
        public string? Twitter { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public bool TwitterVisibility { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string SiteLogo { get; set; }
        public string? Youtube { get; set; }
        public bool YoutubeVisibility { get; set; }

        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string Email { get; set; }

    }
}
