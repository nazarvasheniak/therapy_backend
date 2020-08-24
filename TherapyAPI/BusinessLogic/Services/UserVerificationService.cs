using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Services
{
    public class UserVerificationService : BaseCrudService<UserVerification>, IUserVerificationService
    {
        public UserVerificationService(IRepository<UserVerification> repository) : base(repository)
        {
        }

        public UserVerification GetUserVerification(User user)
        {
            return GetAll().FirstOrDefault(x => x.User == user);
        }
    }
}
