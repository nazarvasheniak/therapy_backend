using System;
namespace Domain.ViewModels.Request
{
    public class SignUpRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Problem { get; set; }
    }
}
