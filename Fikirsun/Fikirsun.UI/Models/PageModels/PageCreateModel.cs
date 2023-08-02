using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class PageCreateModel
    {
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string Content { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string Title { get; set; }
    }
}
