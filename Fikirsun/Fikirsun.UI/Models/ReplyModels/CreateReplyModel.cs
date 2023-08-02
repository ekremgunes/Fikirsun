using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class CreateReplyModel
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public int commentId { get; set; }
        public int? replyUserId { get; set; }
    }
}
