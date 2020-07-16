using System;
namespace Utils.SberbankAcquiring.Models.Response
{
    public class RegisterDOResponse
    {
        public string OrderId { get; set; }
        public string FormUrl { get; set; }
        public long ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
