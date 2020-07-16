using System;
namespace Utils.SberbankAcquiring.Models
{
    public class RegisterDOWebhook
    {
        public string OrderId { get; set; }
        public long SessionId { get; set; }
    }
}
