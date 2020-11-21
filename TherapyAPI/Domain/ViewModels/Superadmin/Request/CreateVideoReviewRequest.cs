using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Superadmin.Request
{
    public class CreateVideoReviewRequest
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Text { get; set; }

        [Required]
        public long VideoID { get; set; }
    }
}
