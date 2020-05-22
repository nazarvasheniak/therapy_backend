using System;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IUserService : IBaseCrudService<User>
    {
        User FindByPhoneNumber(string phoneNumber);
        User FindByEmail(string email);
    }
}
