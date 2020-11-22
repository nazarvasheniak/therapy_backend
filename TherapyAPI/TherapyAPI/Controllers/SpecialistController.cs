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
using TherapyAPI.BackgroundServices;
using Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TherapyAPI.Controllers
{
    [Route("api/specialist")]
    [Authorize(Roles = "Specialist")]
    public class SpecialistController : Controller
    {
        private IUserService UserService { get; set; }
        private IArticleLikeService ArticleLikeService { get; set; }
        private IArticleCommentService ArticleCommentService { get; set; }
        private IArticlePublishService ArticlePublishService { get; set; }
        private ISpecialistService SpecialistService { get; set; }
        private IReviewService ReviewService { get; set; }
        private ISessionService SessionService { get; set; }
        private IProblemService ProblemService { get; set; }
        private IProblemImageService ProblemImageService { get; set; }
        private IProblemResourceService ProblemResourceService { get; set; }
        private IProblemResourceTaskService ProblemResourceTaskService { get; set; }

        private SessionsTimerService SessionsTimerService { get; set; }

        public SpecialistController([FromServices]
            IUserService userService,
            IArticleLikeService articleLikeService,
            IArticleCommentService articleCommentService,
            IArticlePublishService articlePublishService,
            ISpecialistService specialistService,
            IReviewService reviewService,
            ISessionService sessionService,
            IProblemService problemService,
            IProblemImageService problemImageService,
            IProblemResourceService problemResourceService,
            IProblemResourceTaskService problemResourceTaskService,
            SessionsTimerService sessionsTimerService)
        {
            UserService = userService;
            ArticleLikeService = articleLikeService;
            ArticleCommentService = articleCommentService;
            ArticlePublishService = articlePublishService;
            SpecialistService = specialistService;
            ReviewService = reviewService;
            SessionService = sessionService;
            ProblemService = problemService;
            ProblemImageService = problemImageService;
            ProblemResourceService = problemResourceService;
            ProblemResourceTaskService = problemResourceTaskService;
            SessionsTimerService = sessionsTimerService;
        }

        private SpecialistViewModel GetFullSpecialist(Specialist specialist)
        {
            var reviews = ReviewService.GetAll()
                .Where(x => x.Session.Specialist == specialist)
                .OrderByDescending(x => x.Session.Date)
                .Select(x => new ReviewViewModel(x))
                .ToList();

            var rating = ReviewService.GetSpecialistRating(specialist);

            return new SpecialistViewModel(specialist, rating, reviews);
        }

        private ClientCardViewModel GetClientCard(User user, Specialist specialist)
        {
            var result = new ClientCardViewModel
            {
                ID = user.ID,
                User = new UserViewModel(user),
                ProblemsCount = ProblemService.GetUserProblemsCount(user),
            };

            var sessions = SessionService.GetAll()
                .Where(x => x.Problem.User == user && x.Specialist == specialist)
                .OrderByDescending(x => x.Date)
                .ToList();

            var reviews = new List<ReviewViewModel>();

            sessions.ForEach(session =>
            {
                var review = ReviewService.GetSessionReview(session);
                reviews.Add(new ReviewViewModel(review));
            });

            result.Sessions = sessions.Select(x => GetSpecialistSession(x)).ToList();
            result.AverageScore = (reviews.Sum(x => x.Score) / reviews.Count);
            result.Paid = sessions.Where(x => x.Status == SessionStatus.Success).Sum(x => x.Reward);
            result.RefundsCount = sessions.Where(x => x.Status == SessionStatus.Refund).ToList().Count;

            return result;
        }

        private SpecialistProfileActiveSessionViewModel GetSessionClientCard(Session session)
        {
            return new SpecialistProfileActiveSessionViewModel
            {
                Session = new SessionViewModel(session),
                Client = GetClientCard(session.Problem.User, session.Specialist),
                ImagesCount = ProblemImageService.GetProblemImages(session.Problem).Count,
                ResourcesCount = ProblemResourceService.GetProblemResources(session.Problem).Count
            };
        }

        private SpecialistSessionViewModel GetSpecialistSession(Session session)
        {
            var images = ProblemImageService.GetProblemImages(session.Problem);
            var resources = ProblemResourceService.GetProblemResources(session.Problem);

            var result = new SpecialistSessionViewModel
            {
                SessionID = session.ID,
                SessionDate = session.Date,
                SessionStatus = session.Status,
                Client = new UserViewModel(session.Problem.User),
                Problem = new ProblemViewModel(session.Problem),
                ProblemText = session.Problem.ProblemText,
                Reward = session.Reward,
                Specialist = GetFullSpecialist(session.Specialist),
                IsSpecialistClose = session.IsSpecialistClose,
                IsClientClose = session.IsClientClose,
                SpecialistCloseDate = session.SpecialistCloseDate,
                ClientCloseDate = session.ClientCloseDate,
                SessionImagesCount = images.Where(x => x.Session == session).Count(),
                TotalImagesCount = images.Count,
                SessionResourcesCount = resources.Where(x => x.Session == session).Count(),
                TotalResourcesCount = resources.Count
            };

            var review = ReviewService.GetSessionReview(session);
            if (review != null)
            {
                result.Review = new ReviewViewModel(review);
                result.ReviewScore = review.Score;
            }

            images.ForEach(image =>
            {
                if (image.Session.Specialist != session.Specialist)
                    result.IsAllImagesFromOneSpecialist = false;
            });

            resources.ForEach(resource =>
            {
                if (resource.Session.Specialist != session.Specialist)
                    result.IsAllResourcesFromOneSpecialist = false;
            });

            return result;
        }

        private ProblemResourceViewModel GetFullProblemResource(ProblemResource resource)
        {
            var tasks = ProblemResourceTaskService.GetResourceTasks(resource);

            return new ProblemResourceViewModel(resource, tasks);
        }

        private ArticleViewModel GetFullArticle(Article article)
        {
            var likes = ArticleLikeService.GetArticleLikes(article);
            var comments = ArticleCommentService.GetArticleComments(article);

            return new ArticleViewModel(article, likes, comments);
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
                .OrderByDescending(x => x.Session.Date)
                .Select(x => new ReviewViewModel(x))
                .ToList();

            var sessions = SessionService.GetSpecialistSessions(specialist)
                .Where(x => x.IsClientClose && x.IsSpecialistClose)
                .ToList();

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
                PhotoUrl = specialist.User.Photo != null ? specialist.User.Photo.Url : null,
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

            var sessions = SessionService.GetSpecialistSessions(specialist)
                .Where(x => x.Status != SessionStatus.Waiting)
                .Select(x => GetSpecialistSession(x))
                .ToList();

            var response = PaginationHelper.PaginateEntityCollection(sessions, query);

            return Ok(response);
        }

        [HttpGet("sessions/{sessionID}")]
        public IActionResult GetSession(long sessionID)
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

            var session = SessionService.GetSpecialistSessions(specialist)
                .FirstOrDefault(x => x.ID == sessionID);

            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Сессия не найдена"
                });

            return Ok(new DataResponse<SpecialistSessionViewModel>
            {
                Data = GetSpecialistSession(session)
            });
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
                .Where(x => x.Status == SessionStatus.Started && !x.IsSpecialistClose)
                .Select(x => GetSessionClientCard(x))
                .ToList();

            return Ok(new DataResponse<List<SpecialistProfileActiveSessionViewModel>>
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
                PositiveReviews = ReviewService.GetSpecialistReviews(specialist, "Positive"),
                NeutralReviews = ReviewService.GetSpecialistReviews(specialist, "Neutral"),
                NegativeReviews = ReviewService.GetSpecialistReviews(specialist, "Negative"),
                Rating = ReviewService.GetSpecialistRating(specialist)
            });
        }

        [HttpGet("articles")]
        public IActionResult GetArticles([FromQuery] GetList query)
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

            var publishes = ArticlePublishService.GetAll()
                .Where(publish => publish.Article.Author == specialist)
                .Select(publish => new ArticlePublishViewModel(publish, GetFullArticle(publish.Article)))
                .ToList();

            var response = PaginationHelper.PaginateEntityCollection(publishes, query);

            return Ok(response);
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
                    Resources = ProblemResourceService.GetProblemResources(problem).Select(x => GetFullProblemResource(x)).ToList(),
                    Sessions = SessionService.GetProblemSessions(problem).Select(x => GetSpecialistSession(x)).ToList()
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

            var session = SessionService.GetCurrentSession(problem);
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

            var session = SessionService.GetCurrentSession(problem);
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

            if (request.ParentImageID != 0 && request.ParentImageID != parentImageID)
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
            else if (request.ParentImageID == 0)
            {
                problemImage.ParentImage = null;
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

            var session = SessionService.GetCurrentSession(problem);
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

            var session = SessionService.GetCurrentSession(problem);
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

        [HttpPost("clients/{clientID}/problems/{problemID}/resources")]
        public IActionResult CreateClientProblemResource([FromBody] CreateUpdateProblemResourceRequest request, long clientID, long problemID)
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

            var session = SessionService.GetCurrentSession(problem);
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

            var resource = new ProblemResource
            {
                Session = session,
                Title = request.Title,
                Emotion = request.Emotion,
                Location = request.Location,
                Characteristic = request.Characteristic,
                Influence = request.Influence,
                LikeScore = request.LikeScore
            };

            ProblemResourceService.Create(resource);

            request.Tasks.ForEach(task => ProblemResourceTaskService.CreateTask(task, resource));

            var resources = ProblemResourceService.GetProblemResources(problem)
                .Select(x => GetFullProblemResource(x))
                .ToList();

            return Ok(new DataResponse<List<ProblemResourceViewModel>>
            {
                Data = resources
            });
        }

        [HttpPut("clients/{clientID}/problems/{problemID}/resources/{resourceID}")]
        public IActionResult EditClientProblemResource([FromBody] CreateUpdateProblemResourceRequest request, long clientID, long problemID, long resourceID)
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

            var session = SessionService.GetCurrentSession(problem);
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

            var resource = ProblemResourceService.Get(resourceID);
            if (resource == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Ресурс не найден"
                });

            if (request.Title != resource.Title)
                resource.Title = request.Title;

            if (request.Emotion != resource.Emotion)
                resource.Emotion = request.Emotion;

            if (request.Location != resource.Location)
                resource.Location = request.Location;

            if (request.Characteristic != resource.Characteristic)
                resource.Characteristic = request.Characteristic;

            if (request.Influence != resource.Influence)
                resource.Influence = request.Influence;

            if (request.LikeScore != resource.LikeScore)
                resource.LikeScore = request.LikeScore;

            request.Tasks.ForEach(task => ProblemResourceTaskService.CreateUpdateTask(task, resource));

            var resources = ProblemResourceService.GetProblemResources(problem)
                .Select(x => GetFullProblemResource(x))
                .ToList();

            return Ok(new DataResponse<List<ProblemResourceViewModel>>
            {
                Data = resources
            });
        }
    
        [HttpPost("clients/{clientID}/problems/{problemID}/sessions/{sessionID}/close")]
        public IActionResult CloseClientSession(long clientID, long problemID, long sessionID)
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

            var session = SessionService.Get(sessionID);
            if (session == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Сессия не найдена"
                });

            session.SpecialistCloseDate = DateTime.UtcNow;
            session.IsSpecialistClose = true;
            SessionService.Update(session);

            SessionsTimerService.AddSession(session);

            var sessions = SessionService.GetSpecialistSessions(specialist)
                .Where(x => x.Status == SessionStatus.Started && !x.IsSpecialistClose)
                .Select(x => GetSessionClientCard(x))
                .ToList();

            return Ok(new DataResponse<List<SpecialistProfileActiveSessionViewModel>>
            {
                Data = sessions
            });
        }
    }
}
