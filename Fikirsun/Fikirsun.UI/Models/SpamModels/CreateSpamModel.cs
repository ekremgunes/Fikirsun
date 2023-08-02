using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class CreateSpamModel
    {
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string Name { get; set; }
    }
}
