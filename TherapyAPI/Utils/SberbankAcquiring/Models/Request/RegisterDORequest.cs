using BusinessLogic.Config;
using System;
namespace Utils.SberbankAcquiring.Models.Request
{
    public class RegisterDORequest
    {
        public string UserName { get; set; } = "denischernakov-api";
        public string Password { get; set; } = "denischernakov";
        public string OrderNumber { get; set; }
        public long Amount { get; set; }
        public long Currency { get; set; } = 643;
        public string ReturnUrl { get; set; }
        public string FailUrl { get; set; }

        public RegisterDORequest(string orderNumber, long amount)
        {
            OrderNumber = orderNumber;
            Amount = amount;
            ReturnUrl = $"{AppSettings.ApiUrl}/payments/success";
            FailUrl = $"{AppSettings.ApiUrl}/payments/fail";
        }

        public RegisterDORequest(string orderNumber, long amount, long sessionID)
        {
            OrderNumber = orderNumber;
            Amount = amount;
            ReturnUrl = $"{AppSettings.ApiUrl}/payments/success?sessionId={sessionID}";
            FailUrl = $"{AppSettings.ApiUrl}/payments/fail?sessionId={sessionID}";
        }

        public void SetSessionID(long sessionID)
        {
            ReturnUrl = $"{AppSettings.ApiUrl}/payments/success?sessionId={sessionID}";
            FailUrl = $"{AppSettings.ApiUrl}/payments/fail?sessionId={sessionID}";
        }
    }
}
