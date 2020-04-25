using System;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IUserService : IBaseCrudService<User>
    {
        User FindUser(string phoneNumber);
    }
}
