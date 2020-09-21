using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Domain.ViewModels.Request
{
    public class ChangeSettingsRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public IFormFile Avatar { get; set; }
    }
}
