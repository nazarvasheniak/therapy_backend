using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class CreateArticleCommentRequest
    {
        [Required]
        public bool IsReply { get; set; }

        [Required]
        public long ParentCommentID { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
