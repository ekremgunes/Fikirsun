using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class AnswerCommentModel
    {
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public int commentId { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public int postId { get; set; }
    }
}
