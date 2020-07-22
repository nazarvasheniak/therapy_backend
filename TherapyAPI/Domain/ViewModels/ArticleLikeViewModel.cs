using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class ArticleLikeViewModel : BaseViewModel
    {
        public UserViewModel Author { get; set; }

        public ArticleLikeViewModel(ArticleLike like)
        {
            if (like != null)
            {
                ID = like.ID;
                Author = new UserViewModel(like.Author);
            }
        }
    }
}
