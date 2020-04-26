using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class SignInConfirmRequest
    {
        [Required]
        public long UserID { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
