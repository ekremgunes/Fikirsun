using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI
{
    public class UpdateSpamModel
    {
        [Required]

        public int Id { get; set; }

        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string Name { get; set; }
    }
}
