using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.ViewModels.Request
{
    public class VerificationRequest
    {
        [Required]
        public IFormFile Document { get; set; }

        [Required]
        public IFormFile Selfie { get; set; }
    }
}
