using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class CreateReviewRequest
    {
        [Required]
        public string ReviewText { get; set; }

        [Required]
        public int Score { get; set; }
    }
}
