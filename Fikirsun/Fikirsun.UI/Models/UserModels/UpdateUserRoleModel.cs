using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class UpdateUserRoleModel
    {
        [Required]
        public int roleId { get; set; }
        [Required]
        public int userId { get; set; }
    }
}
