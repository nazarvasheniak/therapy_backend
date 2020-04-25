using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TherapyAPI.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private IUserService UserService { get; set; }

        public AuthController([FromServices]
            IUserService userService)
        {
            UserService = userService;
        }

        [HttpPost(/)]
    }
}
