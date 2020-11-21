using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class GetFileByUrlRequest
    {
        [Required]
        public string Url { get; set; }
    }
}
