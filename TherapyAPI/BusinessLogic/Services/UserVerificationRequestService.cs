using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Services
{
    public class UserVerificationRequestService : BaseCrudService<UserVerificationRequest>, IUserVerificationRequestService
    {
        public UserVerificationRequestService(IRepository<UserVerificationRequest> repository) : base(repository)
        {
        }

        public UserVerificationRequest GetVerificationRequest(User user)
        {
            return GetAll().FirstOrDefault(x => x.User == user);
        }

        public UserVerificationRequest CreateVerificationRequest(User user, File document, File selfie)
        {
            var verificationRequest = GetVerificationRequest(user);
            if (verificationRequest != null)
                return null;

            verificationRequest = new UserVerificationRequest
            {
                User = user,
                Document = document,
                Selfie = selfie,
                Status = UserVerificationRequestStatus.New
            };

            Create(verificationRequest);

            return verificationRequest;
        }
    }
}
