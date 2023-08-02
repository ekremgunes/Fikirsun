using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class SocialMediaModels
    {
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string ExtraLink { get; set; }

        public string? Instagram { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public bool InstagramVisibilty { get; set; }

        public string? Facebook { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public bool FacebookVisibility { get; set; }

        public string? Twitter { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public bool TwitterVisibility { get; set; }

        public string? Youtube { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]

        public bool YoutubeVisibility { get; set; }


    }
}
