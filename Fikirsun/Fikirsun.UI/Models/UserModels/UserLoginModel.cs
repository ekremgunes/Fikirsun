using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class UserLoginModel
    {
        [Required]

        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        //public string Email { get; set; }

    }
}
