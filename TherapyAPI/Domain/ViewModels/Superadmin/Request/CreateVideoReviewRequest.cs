using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Superadmin.Request
{
    public class CreateVideoReviewRequest
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public long PhotoID { get; set; }

        [Required]
        [MaxLength(120)]
        public string Text { get; set; }

        [Required]
        public string LinkVK { get; set; }

        [Required]
        public string LinkYouTube { get; set; }
    }
}
