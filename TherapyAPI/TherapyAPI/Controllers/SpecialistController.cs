using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels;
using Domain.ViewModels.Request;
using Domain.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TherapyAPI.Controllers
{
    [Route("api/specialist")]
    [Authorize]
    public class SpecialistController : Controller
    {
        private IUserService UserService { get; set; }
        private ISpecialistService SpecialistService { get; set; }
        private IReviewService ReviewService { get; set; }
        private ISessionService SessionService { get; set; }

        public SpecialistController([FromServices]
            IUserService userService,
            ISpecialistService specialistService,
            IReviewService reviewService,
            ISessionService sessionService)
        {
            UserService = userService;
            SpecialistService = specialistService;
            ReviewService = reviewService;
            SessionService = sessionService;
        }

        private SpecialistViewModel GetFullSpecialist(Specialist specialist)
        {
            var reviews = ReviewService.GetAll()
                .Where(x => x.Session.Specialist == specialist)
                .Select(x => new ReviewViewModel(x))
                .ToList();

            var rating = ReviewService.GetSpecialistRating(specialist);

            return new SpecialistViewModel(specialist, rating, reviews);
        }

        [HttpGet("info")]
        public IActionResult GetSpecialistInfo()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var specialist = SpecialistService.GetSpecialistFromUser(user);
            if (specialist == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Специалист не найден"
                });

            return Ok(new DataResponse<SpecialistViewModel>
            {
                Data = GetFullSpecialist(specialist)
            });
        }

        [HttpGet("profile")]
        public IActionResult GetSpecialistProfile()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var specialist = SpecialistService.GetSpecialistFromUser(user);
            if (specialist == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Специалист не найден"
                });

            var reviews = ReviewService.GetAll()
                .Where(x => x.Session.Specialist == specialist)
                .Select(x => new ReviewViewModel(x))
                .ToList();

            var sessions = SessionService.GetSpecialistSessions(specialist);

            var clients = new HashSet<User>();

            sessions.ForEach(session => clients.Add(session.Problem.User));

            var profile = new SpecialistProfileViewModel
            {
                SpecialistID = specialist.ID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Rating = ReviewService.GetSpecialistRating(specialist),
                PositiveReviewsCount = reviews.Where(x => x.Score > 3).Count(),
                NeutralReviewsCount = reviews.Where(x => x.Score == 3).Count(),
                NegativeReviewsCount = reviews.Where(x => x.Score < 3).Count(),
                PhotoUrl = specialist.Photo.Url,
                Price = specialist.Price,
                TotalEarnings = sessions.Where(x => x.Status == SessionStatus.Success).Sum(x => x.Reward),
                TotalSessions = sessions.Count,
                TotalClients = clients.Count
            };

            return Ok(new DataResponse<SpecialistProfileViewModel>
            {
                Data = profile
            });
        }

        [HttpPost("price")]
        public IActionResult ChangePrice([FromBody] ChangeSpecialistPriceRequest request)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var specialist = SpecialistService.GetSpecialistFromUser(user);
            if (specialist == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Специалист не найден"
                });

            specialist.Price = request.Price;
            SpecialistService.Update(specialist);

            return Ok(new ResponseModel());
        }

        [HttpGet("sessions/active")]
        public IActionResult GetActiveSession()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var specialist = SpecialistService.GetSpecialistFromUser(user);
            if (specialist == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Специалист не найден"
                });

            var sessions = SessionService.GetSpecialistSessions(specialist)
                .Where(x => x.Status == SessionStatus.Started)
                .Select(x => new SessionViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<SessionViewModel>>
            {
                Data = sessions
            });
        }
    }
}
