using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.Models;
using Domain.ViewModels;
using Domain.ViewModels.Request;
using Domain.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TherapyAPI.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class UsersController : Controller
    {
        private IUserService UserService { get; set; }
        private IUserVerificationService UserVerificationService { get; set; }
        private IUserVerificationRequestService UserVerificationRequestService { get; set; }
        private IFileService FileService { get; set; }

        public UsersController([FromServices]
            IUserService userService,
            IUserVerificationService verificationService,
            IUserVerificationRequestService verificationRequestService,
            IFileService fileService)
        {
            UserService = userService;
            UserVerificationService = verificationService;
            UserVerificationRequestService = verificationRequestService;
            FileService = fileService;
        }

        [HttpGet("info")]
        public IActionResult GetUserInfo()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            return Ok(new DataResponse<UserViewModel>
            {
                Data = new UserViewModel(user)
            });
        }

        [HttpGet("verification")]
        public IActionResult GetUserVerification()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var verification = UserVerificationService.GetUserVerification(user);
            if (verification == null)
                return Ok(new GetVerificationResponse
                {
                    IsVerified = false
                });

            return Ok(new GetVerificationResponse
            {
                IsVerified = true
            });
        }

        [HttpGet("verification/request")]
        public IActionResult GetUserVerificationRequest()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var verificationRequest = UserVerificationRequestService.GetVerificationRequest(user);
            if (verificationRequest == null)
                return Ok(new DataResponse<UserVerificationRequestViewModel>
                {
                    Data = null
                });

            return Ok(new DataResponse<UserVerificationRequestViewModel>
            {
                Data = new UserVerificationRequestViewModel(verificationRequest)
            });
        }

        [HttpPost("verification/request")]
        public async Task<IActionResult> VerificationRequest([FromForm] VerificationRequest request)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var verificationRequest = UserVerificationRequestService.GetVerificationRequest(user);
            if (verificationRequest != null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Вы уже отправили запрос на верификацию"
                });

            var document = await FileService.SaveFileForm(request.Document);
            var selfie = await FileService.SaveFileForm(request.Selfie);

            UserVerificationRequestService.CreateVerificationRequest(user, document, selfie);

            return Ok(new ResponseModel());
        }
    }
}
