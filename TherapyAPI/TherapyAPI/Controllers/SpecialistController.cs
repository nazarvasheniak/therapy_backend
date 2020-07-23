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
using Utils;

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
        private IProblemService ProblemService { get; set; }

        public SpecialistController([FromServices]
            IUserService userService,
            ISpecialistService specialistService,
            IReviewService reviewService,
            ISessionService sessionService,
            IProblemService problemService)
        {
            UserService = userService;
            SpecialistService = specialistService;
            ReviewService = reviewService;
            SessionService = sessionService;
            ProblemService = problemService;
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

        public ClientCardViewModel GetClientCard(User user, Specialist specialist)
        {
            var result = new ClientCardViewModel
            {
                User = new UserViewModel(user),
                ProblemsCount = ProblemService.GetUserProblemsCount(user),
            };

            var sessions = SessionService.GetAll()
                    .Where(x => x.Problem.User == user && x.Specialist == specialist)
                    .ToList();

            var reviews = new List<ReviewViewModel>();

            sessions.ForEach(session =>
            {
                var review = ReviewService.GetSessionReview(session);
                reviews.Add(new ReviewViewModel(review));
            });

            result.Sessions = sessions.Select(x => new SessionViewModel(x)).ToList();
            result.AverageScore = (reviews.Sum(x => x.Score) / reviews.Count);
            result.Paid = sessions.Where(x => x.Status == SessionStatus.Success).Sum(x => x.Reward);
            result.RefundsCount = sessions.Where(x => x.Status == SessionStatus.Refund).ToList().Count;

            return result;
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

        [HttpGet("clients")]
        public IActionResult GetClients([FromQuery] GetList query)
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

            var clients = SessionService.GetSpecialistClients(specialist);
            var response = PaginationHelper.PaginateEntityCollection(clients.Select(client => GetClientCard(client, specialist)), query);

            return Ok(response);
        }

        [HttpGet("clients/{id}")]
        public IActionResult GetClient(long id)
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

            var client = UserService.Get(id);
            if (client == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Клиент не найден"
                });

            var clientCard = GetClientCard(client, specialist);

            return Ok(new DataResponse<ClientCardViewModel>
            {
                Data = clientCard
            });
        }
    }
}
