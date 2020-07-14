using System;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IUserWalletService : IBaseCrudService<UserWallet>
    {
        UserWallet GetUserWallet(User user);
    }
}
