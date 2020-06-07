using System;
namespace Domain.ViewModels
{
    public class ArticleViewModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public FileViewModel Image { get; set; }
        public string ShortText { get; set; }
        public string Text { get; set; }
        public UserViewModel Author { get; set; }
        public DateTime Date { get; set; }
    }
}
