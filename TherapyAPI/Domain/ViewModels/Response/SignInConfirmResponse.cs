using System;
using Domain.Enums;

namespace Domain.ViewModels.Response
{
    public class SignInConfirmResponse : ResponseModel
    {
        public string Token { get; set; }
        public UserRole Role { get; set; }
    }
}
