using System;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class UserService : BaseCrudService<User>, IUserService
    {
        public UserService(IRepository<User> repository) : base(repository)
        {
        }

        public User FindByPhoneNumber(string phoneNumber)
        {
            return GetAll().FirstOrDefault(x => x.PhoneNumber == phoneNumber);
        }

        public User FindByEmail(string email)
        {
            return GetAll().FirstOrDefault(x => x.Email == email);
        }
    } 
}
