using System;
using Domain.Enums;

namespace Domain.ViewModels.Superadmin.Request
{
    public class UpdateArticlePublishRequest
    {
        public ArticleModerationStatus Status { get; set; }
        public string Message { get; set; }
    }
}
