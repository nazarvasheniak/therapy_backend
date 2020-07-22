using System;
using Domain.Models;

namespace Domain.ViewModels
{
    public class UserWalletViewModel : BaseViewModel
    {
        public UserViewModel User { get; set; }
        public double Balance { get; set; }
        public double LockedBalance { get; set; }

        public UserWalletViewModel(UserWallet wallet, double lockedBalance)
        {
            if (wallet != null)
            {
                ID = wallet.ID;
                User = new UserViewModel(wallet.User);
                Balance = wallet.Balance;
                LockedBalance = lockedBalance;
            }
        }
    }
}
