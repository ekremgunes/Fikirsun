using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class UserSignupModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
