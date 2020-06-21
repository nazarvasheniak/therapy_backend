using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class CreateUpdateArticleRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string ShortText { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public long PreviewImageID { get; set; }
    }
}
