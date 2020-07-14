using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class UserWalletService : BaseCrudService<UserWallet>, IUserWalletService
    {
        public UserWalletService(IRepository<UserWallet> repository) : base(repository)
        {
        }

        public UserWallet GetUserWallet(User user)
        {
            return GetAll().FirstOrDefault(x => x.User == user);
        }
    }
}
