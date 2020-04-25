using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.ViewModels.Request;
using Domain.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TherapyAPI.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private IUserService UserService { get; set; }
        private IUserSessionService UserSessionService { get; set; }

        public AuthController([FromServices]
            IUserService userService,
            IUserSessionService userSessionService)
        {
            UserService = userService;
            UserSessionService = userSessionService;
        }

        // api/auth/sign-in
        [HttpPost("sign-in")]
        public IActionResult SignIn([FromBody] SignInRequest request)
        {
            var user = UserService.FindUser(request.PhoneNumber);
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            UserSessionService.CreateSession(user);

            return Ok(new ResponseModel());
        }
    }
}
