using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ViewModels
{
    public class UserVerificationViewModel : BaseViewModel
    {
        public UserViewModel User { get; set; }
        public FileViewModel Document { get; set; }
        public FileViewModel Selfie { get; set; }
        public UserVerificationRequestViewModel VerificationRequest { get; set; }

        public UserVerificationViewModel(UserVerification verification)
        {
            if (verification != null)
            {
                User = new UserViewModel(verification.User);
                Document = new FileViewModel(verification.Document);
                Selfie = new FileViewModel(verification.Selfie);
                VerificationRequest = new UserVerificationRequestViewModel(verification.VerificationRequest);
            }
        }
    }
}
