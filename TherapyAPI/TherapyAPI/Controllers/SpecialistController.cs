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
        private IProblemImageService ProblemImageService { get; set; }
        private IProblemResourceService ProblemResourceService { get; set; }
        private IProblemResourceTaskService ProblemResourceTaskService { get; set; }

        public SpecialistController([FromServices]
            IUserService userService,
            ISpecialistService specialistService,
            IReviewService reviewService,
            ISessionService sessionService,
            IProblemService problemService,
            IProblemImageService problemImageService,
            IProblemResourceService problemResourceService,
            IProblemResourceTaskService problemResourceTaskService)
        {
            UserService = userService;
            SpecialistService = specialistService;
            ReviewService = reviewService;
            SessionService = sessionService;
            ProblemService = problemService;
            ProblemImageService = problemImageService;
            ProblemResourceService = problemResourceService;
            ProblemResourceTaskService = problemResourceTaskService;
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

        private ClientCardViewModel GetClientCard(User user, Specialist specialist)
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

        private SpecialistSessionViewModel GetSpecialistSession(Session session, Review review = null)
        {
            var result = new SpecialistSessionViewModel
            {
                SessionID = session.ID,
                SessionDate = session.Date,
                SessionStatus = session.Status,
                Client = new UserViewModel(session.Problem.User),
                ProblemText = session.Problem.ProblemText,
                Reward = session.Reward
            };

            if (review != null)
                result.ReviewScore = review.Score;

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

        [HttpGet("clients/{clientID}")]
        public IActionResult GetClient(long clientID)
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

            var client = UserService.Get(clientID);
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

        [HttpGet("clients/{clientID}/problems/{problemID}/assets")]
        public IActionResult GetClientAssets(long clientID, long problemID)
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

            var client = UserService.Get(clientID);
            if (client == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Клиент не найден"
                });

            var problem = ProblemService.Get(problemID);
            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            if (problem.User != client)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            return Ok(new DataResponse<ProblemAssetsViewModel>
            {
                Data = new ProblemAssetsViewModel
                {
                    Problem = new ProblemViewModel(problem),
                    Images = ProblemImageService.GetProblemImages(problem).Select(x => new ProblemImageViewModel(x)).ToList(),
                    Resources = ProblemResourceService.GetProblemResources(problem).Select(x => new ProblemResourceViewModel(x)).ToList(),
                    Sessions = SessionService.GetSpecialistSessions(specialist).Where(x => x.Problem.User == client).Select(x => GetSpecialistSession(x)).ToList()
                }
            });
        }

        [HttpPost("clients/{clientID}/problems/{problemID}/images")]
        public IActionResult CreateClientProblemImage([FromBody] CreateUpdateProblemImageRequest request, long clientID, long problemID)
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

            var client = UserService.Get(clientID);
            if (client == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Клиент не найден"
                });

            var problem = ProblemService.Get(problemID);
            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            if (problem.User != client)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var session = SessionService.GetCurrentSession(client, specialist);
            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Активная сессия не найдена"
                });

            if (session.Problem != problem)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var problemImage = new ProblemImage
            {
                Session = session,
                Title = request.Title,
                Emotion = request.Emotion,
                Characteristic = request.Characteristic,
                Location = request.Location,
                LikeScore = request.LikeScore,
                IsMine = request.IsMine,
                IsIDo = request.IsIDo,
                IsForever = request.IsForever,
                IsHidden = false
            };

            if (request.ParentImageID != 0)
            {
                var parentImage = ProblemImageService.Get(request.ParentImageID);
                if (parentImage == null)
                    return NotFound(new ResponseModel
                    {
                        Success = false,
                        Message = "Зависимость не найдена"
                    });

                problemImage.ParentImage = parentImage;
            }

            ProblemImageService.Create(problemImage);

            var problemImages = ProblemImageService.GetProblemImages(problem)
                .Select(x => new ProblemImageViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<ProblemImageViewModel>>
            {
                Data = problemImages
            });
        }

        [HttpPut("clients/{clientID}/problems/{problemID}/images/{imageID}")]
        public IActionResult EditClientProblemImage([FromBody] CreateUpdateProblemImageRequest request, long clientID, long problemID, long imageID)
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

            var client = UserService.Get(clientID);
            if (client == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Клиент не найден"
                });

            var problem = ProblemService.Get(problemID);
            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            if (problem.User != client)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var session = SessionService.GetCurrentSession(client, specialist);
            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Активная сессия не найдена"
                });

            if (session.Problem != problem)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var problemImage = ProblemImageService.Get(imageID);
            if (problemImage == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Образ не найден"
                });

            if (request.Title != problemImage.Title)
                problemImage.Title = request.Title;

            if (request.Emotion != problemImage.Emotion)
                problemImage.Emotion = request.Emotion;

            if (request.Location != problemImage.Location)
                problemImage.Location = request.Location;

            if (request.Characteristic != problemImage.Characteristic)
                problemImage.Characteristic = request.Characteristic;

            if (request.IsMine != problemImage.IsMine)
                problemImage.IsMine = request.IsMine;

            if (request.IsIDo != problemImage.IsIDo)
                problemImage.IsIDo = request.IsIDo;

            if (request.IsForever != problemImage.IsForever)
                problemImage.IsForever = request.IsForever;

            if (request.LikeScore != problemImage.LikeScore)
                problemImage.LikeScore = request.LikeScore;

            long parentImageID = 0;
            if (problemImage.ParentImage != null)
                parentImageID = problemImage.ParentImage.ID;

            if (request.ParentImageID != parentImageID)
            {
                var newParentImage = ProblemImageService.Get(request.ParentImageID);
                if (newParentImage == null)
                    return NotFound(new ResponseModel
                    {
                        Success = false,
                        Message = "Зависимость не найдена"
                    });

                problemImage.ParentImage = newParentImage;
            }

            ProblemImageService.Update(problemImage);

            var problemImages = ProblemImageService.GetProblemImages(problem)
                .Select(x => new ProblemImageViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<ProblemImageViewModel>>
            {
                Data = problemImages
            });
        }

        [HttpPatch("clients/{clientID}/problems/{problemID}/images/{imageID}")]
        public IActionResult ReloadClientProblemImage(long clientID, long problemID, long imageID)
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

            var client = UserService.Get(clientID);
            if (client == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Клиент не найден"
                });

            var problem = ProblemService.Get(problemID);
            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            if (problem.User != client)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var image = ProblemImageService.Get(imageID);
            if (image == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Образ не найден"
                });

            var session = SessionService.GetCurrentSession(client, specialist);
            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Активная сессия не найдена"
                });

            if (session.Problem != problem)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            image.IsHidden = false;
            ProblemImageService.Update(image);

            var problemImages = ProblemImageService.GetProblemImages(problem)
                .Select(x => new ProblemImageViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<ProblemImageViewModel>>
            {
                Data = problemImages
            });
        }

        [HttpDelete("clients/{clientID}/problems/{problemID}/images/{imageID}")]
        public IActionResult HideCliendProblemImage(long clientID, long problemID, long imageID)
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

            var client = UserService.Get(clientID);
            if (client == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Клиент не найден"
                });

            var problem = ProblemService.Get(problemID);
            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            if (problem.User != client)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var image = ProblemImageService.Get(imageID);
            if (image == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Образ не найден"
                });

            var session = SessionService.GetCurrentSession(client, specialist);
            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Активная сессия не найдена"
                });

            if (session.Problem != problem)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            image.IsHidden = true;
            ProblemImageService.Update(image);

            var problemImages = ProblemImageService.GetProblemImages(problem)
                .Select(x => new ProblemImageViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<ProblemImageViewModel>>
            {
                Data = problemImages
            });
        }

        [HttpGet("sessions")]
        public IActionResult GetSessions([FromQuery] GetList query)
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

            var sessions = SessionService.GetSpecialistSessions(specialist).ToList();
            var result = new List<SpecialistSessionViewModel>();

            sessions.ForEach(session =>
            {
                var review = ReviewService.GetSessionReview(session);
                result.Add(GetSpecialistSession(session, review));
            });

            var response = PaginationHelper.PaginateEntityCollection(result, query);

            return Ok(response);
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

        [HttpGet("reviews")]
        public IActionResult GetReviews()
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

            return Ok(new ReviewsResponse
            {
                PositiveReviews = ReviewService.GetSpecialistReviews(specialist, ReviewType.Positive),
                NeutralReviews = ReviewService.GetSpecialistReviews(specialist, ReviewType.Neutral),
                NegativeReviews = ReviewService.GetSpecialistReviews(specialist, ReviewType.Negative),
                Rating = ReviewService.GetSpecialistRating(specialist)
            });
        }
    }
}
