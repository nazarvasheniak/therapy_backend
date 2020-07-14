using System;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class UserWalletMap : ClassMap<UserWallet>
    {
        public UserWalletMap()
        {
            Table("users_wallets");
            Id(u => u.ID, "id");

            References(e => e.User, "id_user");

            Map(u => u.Balance, "balance").Default("0");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
