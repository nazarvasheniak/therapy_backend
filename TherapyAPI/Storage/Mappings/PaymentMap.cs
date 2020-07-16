using System;
using Domain.Enums;
using Domain.Models;
using FluentNHibernate.Mapping;

namespace Storage.Mappings
{
    public class PaymentMap : ClassMap<Payment>
    {
        public PaymentMap()
        {
            Table("payments");
            Id(u => u.ID, "id");

            References(e => e.Wallet, "id_user_wallet");

            Map(u => u.OrderID, "order_id");
            Map(u => u.Amount, "amount");
            Map(u => u.Type, "payment_type").CustomType<PaymentType>();
            Map(u => u.Status, "payment_status").CustomType<PaymentStatus>();
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
