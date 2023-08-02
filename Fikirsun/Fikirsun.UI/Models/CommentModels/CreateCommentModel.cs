using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models.CommentModels
{
    public class CreateCommentModel
    {
        [Required(ErrorMessage = "Bu alan zorunlu")]

        public string Content { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]

        public int postId { get; set; }
    }
}
