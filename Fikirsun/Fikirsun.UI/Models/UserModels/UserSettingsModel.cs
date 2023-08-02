using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class UserSettingsModel
    {
        public string? UserSubName { get; set; }
        [Required(ErrorMessage = "Zorunlu Alan")]

        public string Email { get; set; }

        public string? About { get; set; }

        [Required(ErrorMessage = "Zorunlu Alan")]
        public string UserName { get; set; }

        public string? SocialMedia { get; set; }
        public string? ExtraLink { get; set; }
        public string? ProfilePhoto { get; set; }

        public bool IsDeleted { get; set; } = false;

        public string? NewPassword { get; set; }
    }
}
