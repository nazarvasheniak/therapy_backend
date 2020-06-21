using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class UploadFileRequest
    {
        [Required]
        public string Base64string { get; set; }

    }
}
