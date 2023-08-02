using Fikirsun.Entities;
using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class PostUpdateModel
    {
        [Required(ErrorMessage = "Bu alan zorunlu")]

        public int Id { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Bu alan zorunlu")]
        public string postCategory { get; set; }
        public string[]? Tags { get; set; }
        public Category? category { get; set; }
    }
}
