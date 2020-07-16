using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
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
    [Route("api/patient")]
    [Authorize]
    public class PatientController : Controller
    {
        private IUserService UserService { get; set; }
        private IProblemService ProblemService { get; set; }
        private ISessionService SessionService { get; set; }
        private ISpecialistService SpecialistService { get; set; }
        private IReviewService ReviewService { get; set; }
        private IUserWalletService UserWalletService { get; set; }

        public PatientController([FromServices]
            IUserService userService,
            IProblemService problemService,
            ISessionService sessionService,
            ISpecialistService specialistService,
            IReviewService reviewService,
            IUserWalletService userWalletService)
        {
            UserService = userService;
            ProblemService = problemService;
            SessionService = sessionService;
            SpecialistService = specialistService;
            ReviewService = ReviewService;
            UserWalletService = userWalletService;
        }

        private SpecialistViewModel GetFullSpecialist(long id)
        {
            var specialist = SpecialistService.Get(id);

            if (specialist == null)
                return null;

            var reviews = ReviewService.GetAll()
                .Where(x => x.Session.Specialist == specialist)
                .Select(x => new ReviewViewModel(x))
                .ToList();

            var rating = ReviewService.GetSpecialistRating(specialist);

            return new SpecialistViewModel(specialist, rating, reviews);
        }

        private SpecialistViewModel GetFullSpecialist(Specialist specialist)
        {
            var test = ReviewService.GetAll();
            var reviews = ReviewService.GetAll()
                .Where(x => x.Session.Specialist == specialist)
                .Select(x => new ReviewViewModel(x))
                .ToList();

            var rating = ReviewService.GetSpecialistRating(specialist);

            return new SpecialistViewModel(specialist, rating, reviews);
        }

        [HttpGet("problems")]
        public IActionResult GetProblems()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problems = ProblemService.GetAll().Where(x => x.User == user).Select(x => new ProblemViewModel(x)).ToList();

            return Ok(new DataResponse<List<ProblemViewModel>>
            {
                Data = problems
            });
        }

        [HttpGet("problems/{id}")]
        public IActionResult GetProblem(long id)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problem = ProblemService.Get(id);

            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            return Ok(new DataResponse<ProblemViewModel>
            {
                Data = new ProblemViewModel(problem)
            });
        }

        [HttpPost("problems")]
        public IActionResult CreateProblem([FromBody] CreateProblemRequest request)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problem = new Problem
            {
                User = user,
                ProblemText = request.ProblemText
            };

            ProblemService.Create(problem);

            return Ok(new DataResponse<ProblemViewModel>
            {
                Data = new ProblemViewModel(problem)
            });
        }

        [HttpGet("problems/{id}/sessions")]
        public IActionResult GetProblemSessions(long id)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problem = ProblemService.Get(id);

            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            if (problem.User != user)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Доступ запрещен"
                });

            var sessions = SessionService.GetAll()
                .Where(x => x.Problem == problem)
                .OrderBy(x => x.Date)
                .Select(x => new SessionViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<SessionViewModel>>
            {
                Data = sessions
            });
        }

        [HttpPost("problems/{id}/sessions")]
        public IActionResult CreateProblemSession([FromBody] CreateSessionRequest request, long id)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problem = ProblemService.Get(id);

            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            var specialist = SpecialistService.Get(request.SpecialistID);

            if (specialist == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Специалист не найден"
                });

            var session = new Session
            {
                Specialist = specialist,
                Problem = problem,
                Reward = specialist.Price,
                Status = SessionStatus.Waiting
            };

            SessionService.Create(session);

            return Ok(new CreateSessionResponse
            {
                SessionID = session.ID
            });
        }

        [HttpPost("problems/{id}/sessions/{sessionID}/start")]
        public IActionResult StartSession(long id, long sessionID)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problem = ProblemService.Get(id);
            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            var session = SessionService.Get(sessionID);
            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Сессия не найдена"
                });

            if (session.Specialist == null)
                return Ok(new ResponseModel
                {
                    Success = false,
                    Message = "Не выбран специалист"
                });

            var wallet = UserWalletService.GetUserWallet(user);
            if (wallet == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Кошелек не найден"
                });

            var activeSessions = SessionService.GetActiveSessions(user);
            var lockedBalance = activeSessions.Sum(x => x.Reward);

            if ((wallet.Balance - lockedBalance) < session.Reward)
                return Ok(new ResponseModel
                {
                    Success = false,
                    Message = "Недостаточно средств"
                });

            session.Status = SessionStatus.Started;
            session.Date = DateTime.UtcNow;

            SessionService.Update(session);

            return Ok(new ResponseModel());
        }

        [HttpPut("problems/{id}/sessions/{sessionID}")]
        public IActionResult ChangeSessionSpecialist([FromBody] CreateSessionRequest request, long id, long sessionID)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problem = ProblemService.Get(id);
            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            var session = SessionService.Get(sessionID);
            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Сессия не найдена"
                });

            var specialist = SpecialistService.Get(request.SpecialistID);
            if (specialist == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Специалист не найден"
                });

            session.Specialist = specialist;
            session.Reward = specialist.Price;
            session.Status = SessionStatus.Waiting;

            SessionService.Update(session);

            return Ok(new ResponseModel());
        }
    }
}
