using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Services
{
    public class UserVerificationService : BaseCrudService<UserVerification>, IUserVerificationService
    {
        public UserVerificationService(IRepository<UserVerification> repository) : base(repository)
        {
        }
    }
}
