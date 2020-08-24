using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Interfaces
{
    public interface IUserVerificationService : IBaseCrudService<UserVerification>
    {
        UserVerification GetUserVerification(User user);
    }
}
