using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels.Request;
using Domain.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
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
        private IProblemService ProblemService { get; set; }
        private IUserWalletService UserWalletService { get; set; }

        public AuthController([FromServices]
            IUserService userService,
            IUserSessionService userSessionService,
            IProblemService problemService,
            IUserWalletService userWalletService)
        {
            UserService = userService;
            UserSessionService = userSessionService;
            ProblemService = problemService;
            UserWalletService = userWalletService;
        }

        [HttpPost("test/sign-in")]
        public IActionResult TestSignIn([FromBody] SignInRequest request)
        {
            var user = UserService.FindByPhoneNumber(request.PhoneNumber);
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Номер телефона не зарегистрирован"
                });

            if (user.PhoneNumber != "+78888888888" && user.PhoneNumber != "+79999999999")
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            if (UserSessionService.GetUserActiveSession(user) != null)
                UserSessionService.CloseUserActiveSession(user);

            UserSessionService.CreateSession(user);

            var token = UserSessionService.AuthorizeUser(user);

            return Ok(new SignInConfirmResponse
            {
                Token = token,
                Role = user.Role
            });
        }

        ///api/auth/sign-up
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
                Role = UserRole.Client
            };

            UserService.Create(user);

            UserWalletService.Create(new UserWallet
            {
                User = user,
                Balance = 0
            });

            ProblemService.Create(new Problem
            {
                User = user,
                ProblemText = request.Problem
            });

            var session = UserSessionService.CreateSession(user);
            SmscHelper.SendSms(user.PhoneNumber, $"Код для входа: {session.AuthCode}");

            return Ok(new SignInResponse
            {
                UserID = user.ID
            });
        }

        ///api/auth/sign-in
        [HttpPost("sign-in")]
        public IActionResult SignIn([FromBody] SignInRequest request)
        {
            var user = UserService.FindByPhoneNumber(request.PhoneNumber);
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Номер телефона не зарегистрирован"
                });

            if (UserSessionService.GetUserActiveSession(user) != null)
                UserSessionService.CloseUserActiveSession(user);

            var session = UserSessionService.CreateSession(user);

            SmscHelper.SendSms(user.PhoneNumber, $"Код для входа: {session.AuthCode}");

            return Ok(new SignInResponse
            {
                UserID = user.ID
            });
        }

        ///api/auth/sign-in/confirm
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
                Token = token,
                Role = user.Role
            });
        }

        ///api/auth/sign-in/confirm/resend
        [HttpPost("sign-in/confirm/resend")]
        public IActionResult ResendConfirmCode([FromBody] ResendConfirmCodeRequest request)
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

            UserSessionService.CloseUserActiveSession(user);
            session = UserSessionService.CreateSession(user);

            SmscHelper.SendSms(user.PhoneNumber, $"Код для входа: {session.AuthCode}");

            return Ok(new SignInResponse
            {
                UserID = user.ID
            });
        }

        ///api/auth/logout
        [HttpDelete("logout")]
        [Authorize]
        public IActionResult UserLogout()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            UserSessionService.CloseUserActiveSession(user);

            return Ok(new ResponseModel());
        }
    }
}
