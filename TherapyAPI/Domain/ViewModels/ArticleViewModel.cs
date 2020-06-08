using System;
using System.Text;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ArticleViewModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public FileViewModel Image { get; set; }
        public string ShortText { get; set; }
        public string Text { get; set; }
        public SpecialistViewModel Author { get; set; }
        public DateTime Date { get; set; }

        public ArticleViewModel(Article article)
        {
            if (article != null)
            {
                ID = article.ID;
                Title = article.Title;
                Image = new FileViewModel(article.Image);
                ShortText = article.ShortText;
                Text = Encoding.UTF8.GetString(article.Text);
                Author = new SpecialistViewModel(article.Author);
                Date = article.Date;
            }
        }
    }
}
