using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels.Request;
using Domain.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using Utils.SmsC;

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

        // api/auth/sign-up
        [HttpPost("sign-up")]
        public IActionResult SignUp([FromBody] SignUpRequest request)
        {
            var user = UserService.FindByPhoneNumber(request.PhoneNumber);
            if (user != null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Номер телефона уже используется"
                });

            user = UserService.FindByEmail(request.Email);
            if (user != null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Email уже используется"
                });

            user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Role = UserRole.Patient
            };

            UserService.Create(user);

            return Ok(new ResponseModel());
        }

        // api/auth/sign-in
        [HttpPost("sign-in")]
        public IActionResult SignIn([FromBody] SignInRequest request)
        {
            var user = UserService.FindByPhoneNumber(request.PhoneNumber);
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            if (UserSessionService.GetUserActiveSession(user) != null)
                UserSessionService.CloseUserActiveSession(user);

            var session = UserSessionService.CreateSession(user);

            var sendSms = SmscHelper.SendSms(user.PhoneNumber, $"Код для входа: {session.AuthCode}");
            Console.WriteLine(sendSms);

            return Ok(new SignInResponse
            {
                UserID = user.ID
            });
        }

        // api/auth/sign-in/confirm
        [HttpPost("sign-in/confirm")]
        public IActionResult SignInConfirm([FromBody] SignInConfirmRequest request)
        {
            var user = UserService.Get(request.UserID);
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var session = UserSessionService.GetUserActiveSession(user);
            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Сессия не найдена"
                });

            if (request.Code != session.AuthCode)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Неверный код подтверждения"
                });

            var token = UserSessionService.AuthorizeUser(user);

            return Ok(new SignInConfirmResponse
            {
                Token = token
            });
        }
    }
}
