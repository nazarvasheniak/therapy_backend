using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ViewModels.Response
{
    public class GetVerificationResponse : ResponseModel
    {
        public bool IsVerified { get; set; }
    }
}
