using System.ComponentModel.DataAnnotations;

namespace Fikirsun.UI.Models
{
    public class CategoryCreateModel
    {
        [Required(ErrorMessage = "Bu alan zorunlu")]

        public string Name { get; set; }
        [Required(ErrorMessage = "Bu alan zorunlu")]
        public short CategoryOrder { get; set; }
    }
}
