using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Interfaces
{
    public interface IUserVerificationRequestService : IBaseCrudService<UserVerificationRequest>
    {
        UserVerificationRequest GetVerificationRequest(User user);
        UserVerificationRequest CreateVerificationRequest(User user, File document, File selfie);
    }
}
