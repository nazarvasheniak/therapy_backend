using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class SignInRequest
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}
