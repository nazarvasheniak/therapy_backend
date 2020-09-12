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
        private IProblemImageService ProblemImageService { get; set; }
        private IProblemResourceService ProblemResourceService { get; set; }
        private IProblemResourceTaskService ProblemResourceTaskService { get; set; }

        public PatientController([FromServices]
            IUserService userService,
            IProblemService problemService,
            ISessionService sessionService,
            ISpecialistService specialistService,
            IReviewService reviewService,
            IUserWalletService userWalletService,
            IProblemImageService problemImageService,
            IProblemResourceService problemResourceService,
            IProblemResourceTaskService problemResourceTaskService)
        {
            UserService = userService;
            ProblemService = problemService;
            SessionService = sessionService;
            SpecialistService = specialistService;
            ReviewService = reviewService;
            UserWalletService = userWalletService;
            ProblemImageService = problemImageService;
            ProblemResourceService = problemResourceService;
            ProblemResourceTaskService = problemResourceTaskService;
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
            var reviews = ReviewService.GetAll()
                .Where(x => x.Session.Specialist == specialist)
                .Select(x => new ReviewViewModel(x))
                .ToList();

            var rating = ReviewService.GetSpecialistRating(specialist);

            return new SpecialistViewModel(specialist, rating, reviews);
        }

        private ProblemResourceViewModel GetFullProblemResource(ProblemResource resource)
        {
            var tasks = ProblemResourceTaskService.GetResourceTasks(resource);

            return new ProblemResourceViewModel(resource, tasks);
        }

        private SessionViewModel GetFullSession(Session session)
        {
            var review = ReviewService.GetSessionReview(session);
            var result = new SessionViewModel(session);

            if (review != null)
                result.ReviewScore = review.Score;

            result.Specialist = GetFullSpecialist(session.Specialist);

            return result;
        }

        [HttpGet("problems")]
        public IActionResult GetProblems()
        {
            if (User.Identity.Name == null)
                return Ok();

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
            if (User.Identity.Name == null)
                return Ok();

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
                .Select(x => GetFullSession(x))
                .ToList();

            return Ok(new DataResponse<List<SessionViewModel>>
            {
                Data = sessions
            });
        }

        [HttpGet("problems/{id}/sessions/{sessionID}")]
        public IActionResult GetSession(long id, long sessionID)
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

            var session = SessionService.Get(sessionID);
            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Сессия не найдена"
                });

            if (session.Problem != problem)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Доступ запрещен"
                });

            return Ok(new DataResponse<SessionViewModel>
            {
                Data = GetFullSession(session)
            });
        }

        [HttpGet("problems/{id}/activeSession")]
        public IActionResult GetProblremActiveSession(long id)
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

            var session = SessionService.GetWaitingSession(problem);
            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Сессия не найдена"
                });

            return Ok(new DataResponse<SessionViewModel>
            {
                Data = GetFullSession(session)
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

        [HttpGet("problems/{problemID}/assets")]
        public IActionResult GetProblemAssets(long problemID)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problem = ProblemService.Get(problemID);
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

            var images = ProblemImageService.GetProblemImages(problem)
                .Select(x => new ProblemImageViewModel(x))
                .ToList();

            var resources = ProblemResourceService.GetProblemResources(problem)
                .Select(x => GetFullProblemResource(x))
                .ToList();

            var sessions = SessionService.GetUserSessions(user)
                .Where(x => x.Problem == problem)
                .Select(x => GetFullSession(x))
                .ToList();

            return Ok(new DataResponse<ClientProblemAssetsViewModel>
            {
                Data = new ClientProblemAssetsViewModel
                {
                    Problem = new ProblemViewModel(problem),
                    Images = images,
                    Resources = resources,
                    Sessions = sessions
                }
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

        [HttpPost("problems/{id}/sessions/{sessionID}/close")]
        public IActionResult CloseSession(long id, long sessionID)
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

            var wallet = UserWalletService.GetUserWallet(user);
            if (wallet == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Кошелек не найден"
                });

            var specialistWallet = UserWalletService.GetUserWallet(session.Specialist.User);
            if (specialistWallet == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Кошелек не найден"
                });

            wallet.Balance -= session.Reward;
            UserWalletService.Update(wallet);

            specialistWallet.Balance += session.Reward;
            UserWalletService.Update(specialistWallet);

            session.IsClientClose = true;
            session.ClientCloseDate = DateTime.UtcNow;
            session.Status = SessionStatus.Success;
            SessionService.Update(session);

            return Ok(new ResponseModel());
        }

        [HttpPost("problems/{id}/sessions/{sessionID}/review")]
        public IActionResult CreateReview([FromBody] CreateReviewRequest request, long id, long sessionID)
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

            var specialist = SpecialistService.Get(session.Specialist.ID);
            session.Specialist = specialist;

            SessionService.Update(session);

            var review = new Review
            {
                Session = session,
                Text = request.ReviewText,
                Score = request.Score
            };

            ReviewService.Create(review);

            return Ok(new ResponseModel());
        }
    }
}
