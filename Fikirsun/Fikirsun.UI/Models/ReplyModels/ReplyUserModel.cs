using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class ReplyUserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string profilePhoto { get; set; }
        public bool isDeleted { get; set; }
    }
}
