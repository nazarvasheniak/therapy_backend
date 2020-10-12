using System;
using Domain.Enums;

namespace Domain.ViewModels.Request
{
    public class ChangeCustomerRoleRequest
    {
        public UserRole Role { get; set; }
    }
}
