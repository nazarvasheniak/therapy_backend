using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.ViewModels;
using Domain.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TherapyAPI.Controllers
{
    [Route("api/wallet")]
    [Authorize]
    public class UsersWalletsController : Controller
    {
        private IUserService UserService { get; set; }
        private IUserWalletService UserWalletService { get; set; }
        private ISessionService SessionService { get; set; }

        public UsersWalletsController([FromServices]
            IUserService userService,
            IUserWalletService userWalletService,
            ISessionService sessionService)
        {
            UserService = userService;
            UserWalletService = userWalletService;
            SessionService = sessionService;
        }

        [HttpGet]
        public IActionResult GetMyWallet()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var wallet = UserWalletService.GetUserWallet(user);
            var activeSessions = SessionService.GetActiveSessions(user);

            return Ok(new DataResponse<UserWalletViewModel>
            {
                Data = new UserWalletViewModel(wallet, activeSessions.Sum(x => x.Reward))
            });
        }
    }
}
