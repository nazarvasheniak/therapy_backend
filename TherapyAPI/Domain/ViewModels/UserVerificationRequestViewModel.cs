using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ViewModels
{
    public class UserVerificationRequestViewModel : BaseViewModel
    {
        public UserViewModel User { get; set; }
        public FileViewModel Document { get; set; }
        public FileViewModel Selfie { get; set; }
        public UserVerificationRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserVerificationRequestViewModel(UserVerificationRequest verificationRequest)
        {
            if (verificationRequest != null)
            {
                User = new UserViewModel(verificationRequest.User);
                Document = new FileViewModel(verificationRequest.Document);
                Selfie = new FileViewModel(verificationRequest.Selfie);
                Status = verificationRequest.Status;
                CreatedAt = verificationRequest.CreatedAt;
                UpdatedAt = verificationRequest.UpdatedAt;
            }
        }
    }
}
