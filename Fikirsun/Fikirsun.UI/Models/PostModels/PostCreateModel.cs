using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class PostCreateModel
    {
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string Category { get; set; }
        public string[]? Tags { get; set; }
    }
}
