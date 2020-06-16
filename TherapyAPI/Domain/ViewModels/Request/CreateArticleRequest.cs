using System;
namespace Domain.ViewModels.Request
{
    public class CreateUpdateArticleRequest
    {
        public string Title { get; set; }
        public string ShortText { get; set; }
        public string Text { get; set; }
        public long PreviewImageID { get; set; }
    }
}
