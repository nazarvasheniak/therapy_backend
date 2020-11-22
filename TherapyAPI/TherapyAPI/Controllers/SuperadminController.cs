using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels;
using Domain.ViewModels.Request;
using Domain.ViewModels.Response;
using Domain.ViewModels.Superadmin;
using Domain.ViewModels.Superadmin.Enums;
using Domain.ViewModels.Superadmin.Request;
using Domain.ViewModels.Superadmin.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TherapyAPI.Controllers
{
    [Route("api/superadmin")]
    [Authorize(Roles = "Administrator")]
    public class SuperadminController : Controller
    {
        private IUserService UserService { get; set; }
        private ISpecialistService SpecialistService { get; set; }
        private ISessionService SessionService { get; set; }
        private IReviewService ReviewService { get; set; }
        private IProblemService ProblemService { get; set; }
        private IProblemImageService ProblemImageService { get; set; }
        private IProblemResourceService ProblemResourceService { get; set; }
        private IClientVideoReviewService VideoReviewService { get; set; }
        private IFileService FileService { get; set; }
        private IArticleService ArticleService { get; set; }
        private IArticleLikeService ArticleLikeService { get; set; }
        private IArticleCommentService ArticleCommentService { get; set; }
        private IArticlePublishService ArticlePublishService { get; set; }

        public SuperadminController([FromServices]
            IUserService userService,
            ISpecialistService specialistService,
            ISessionService sessionService,
            IReviewService reviewService,
            IProblemService problemService,
            IProblemImageService problemImageService,
            IProblemResourceService problemResourceService,
            IClientVideoReviewService videoReviewService,
            IFileService fileService,
            IArticleService articleService,
            IArticleLikeService articleLikeService,
            IArticleCommentService articleCommentService,
            IArticlePublishService articlePublishService)
        {
            UserService = userService;
            SpecialistService = specialistService;
            SessionService = sessionService;
            ReviewService = reviewService;
            ProblemService = problemService;
            ProblemImageService = problemImageService;
            ProblemResourceService = problemResourceService;
            VideoReviewService = videoReviewService;
            FileService = fileService;
            ArticleService = articleService;
            ArticleLikeService = articleLikeService;
            ArticleCommentService = articleCommentService;
            ArticlePublishService = articlePublishService;
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

        private SuperadminPatientModel GetSuperadminPatient(User user)
        {
            var result = new SuperadminPatientModel
            {
                User = new UserViewModel(user),
                AverageScore = ReviewService.GetUserAverageScore(user)
            };

            var sessions = SessionService.GetUserSessions(user).ToList();

            result.TotalSessionsCount = sessions.Count;
            result.TotalRefunds = sessions.Where(x => x.Status == SessionStatus.Refund).ToList().Count;
            result.TotalPaid = sessions.Where(x => x.Status == SessionStatus.Success).Sum(x => x.Reward);

            return result;
        }

        private SuperadminSpecialistModel GetSuperadminSpecialist(User user)
        {
            var specialist = SpecialistService.GetSpecialistFromUser(user);
            if (specialist == null)
                return null;

            var sessions = SessionService.GetSpecialistSessions(specialist)
                .Where(x => x.Status == SessionStatus.Success || x.Status == SessionStatus.Refund)
                .ToList();

            var result = new SuperadminSpecialistModel
            {
                User = new UserViewModel(user),
                Rating = ReviewService.GetSpecialistRating(specialist),
                TotalSessionsCount = sessions.Count,
                TotalEarned = sessions.Where(x => x.Status == SessionStatus.Success).Sum(x => x.Reward)
            };

            return result;
        }

        private SuperadminModel GetSuperadminAdministrator(User user)
        {
            var result = new SuperadminModel
            {
                User = new UserViewModel(user)
            };

            return result;
        }

        private List<SuperadminPatientModel> SortPatientsList(List<SuperadminPatientModel> list, PatientsSorter sortBy, OrderBy orderBy)
        {
            var result = new List<SuperadminPatientModel>();

            switch (sortBy)
            {
                case PatientsSorter.TotalSessionsCount:
                    result.AddRange(list.OrderBy(x => x.TotalSessionsCount));
                    break;

                case PatientsSorter.TotalRefunds:
                    result.AddRange(list.OrderBy(x => x.TotalRefunds));
                    break;

                case PatientsSorter.TotalPaid:
                    result.AddRange(list.OrderBy(x => x.TotalPaid));
                    break;

                case PatientsSorter.AverageScore:
                    result.AddRange(list.OrderBy(x => x.AverageScore));
                    break;

                default:
                    result.AddRange(list.OrderBy(x => x.TotalPaid));
                    break;
            }

            if (orderBy == OrderBy.DESC)
                result.Reverse();

            return result;
        }

        private List<SuperadminSpecialistModel> SortSpecialistList(List<SuperadminSpecialistModel> list, SpecialistsSorter sortBy, OrderBy orderBy)
        {
            var result = new List<SuperadminSpecialistModel>();

            switch (sortBy)
            {
                case SpecialistsSorter.TotalSessionsCount:
                    result.AddRange(list.OrderBy(x => x.TotalSessionsCount));
                    break;

                case SpecialistsSorter.TotalEarned:
                    result.AddRange(list.OrderBy(x => x.TotalEarned));
                    break;

                case SpecialistsSorter.Rating:
                    result.AddRange(list.OrderBy(x => x.Rating));
                    break;

                default:
                    result.AddRange(list.OrderBy(x => x.Rating));
                    break;
            }

            if (orderBy == OrderBy.DESC)
                result.Reverse();

            return result;
        }

        private List<SuperadminModel> SortAdministratorsList(List<SuperadminModel> list, OrderBy orderBy)
        {
            var result = new List<SuperadminModel>();

            result.AddRange(list);

            if (orderBy == OrderBy.DESC)
                result.Reverse();

            return result;
        }

        private IEnumerable<T> FilterListByQueryString<T>(List<T> list, string queryString) where T : SuperadminModel
        {
            foreach (var item in list)
            {
                if (item.User.Email.ToLower().Contains(queryString))
                    yield return item;

                if (item.User.FirstName.ToLower().Contains(queryString))
                    yield return item;

                if (item.User.LastName.ToLower().Contains(queryString))
                    yield return item;

                if (item.User.PhoneNumber.ToLower().Contains(queryString))
                    yield return item;

                if (item.User.RegisteredAt.ToString().Contains(queryString))
                    yield return item;
            }
        }

        private SuperadminCustomerCard GetSuperadminCustomerCard(User user)
        {
            var problems = ProblemService.GetUserProblems(user);
            var sessions = SessionService.GetUserSessions(user)
                .Select(session => GetSpecialistSession(session))
                .ToList();

            return new SuperadminCustomerCard
            {
                UserID = user.ID,
                FullName = $"{user.FirstName} {user.LastName}",
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Sessions = sessions,
                ProblemsCount = problems.Count,

                SpendOrEarned = sessions
                        .Where(session => session.SessionStatus == SessionStatus.Success)
                        .ToList()
                        .Sum(session => session.Reward),

                RefundsCount = sessions
                        .Where(session => session.SessionStatus == SessionStatus.Refund)
                        .ToList()
                        .Count
            };
        }

        private Specialist CreateSpecialist(User user)
        {
            return SpecialistService.CreateSpecialistFromUser(user);
        }

        private List<ArticleViewModel> GetFullArticles(GetList query)
        {
            var isLoggedIn = false;

            User user = null;

            if (User.Identity.Name != null)
            {
                user = UserService.Get(long.Parse(User.Identity.Name));

                if (user != null)
                    isLoggedIn = true;
            }

            var result = new List<ArticleViewModel>();

            var articles = ArticleService.GetAllArticles()
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            articles.ForEach(article =>
            {
                var likes = ArticleLikeService.GetAll()
                    .Where(x => x.Article == article)
                    .ToList();

                var comments = ArticleCommentService.GetAll()
                    .Where(x => x.Article == article)
                    .ToList();

                var isLiked = false;

                if (isLoggedIn)
                {
                    if (likes.FirstOrDefault(x => x.Author == user) != null)
                    {
                        isLiked = true;
                    }
                }

                var articleViewModel = new ArticleViewModel(article, likes, comments, isLiked);
                articleViewModel.Author.Rating = ReviewService.GetSpecialistRating(article.Author);

                result.Add(articleViewModel);
            });

            return result;
        }

        private ArticleViewModel GetFullArticle(long id)
        {
            var article = ArticleService.Get(id);

            if (article == null)
                return null;

            var isLoggedIn = false;

            User user = null;

            if (User.Identity.Name != null)
            {
                user = UserService.Get(long.Parse(User.Identity.Name));

                if (user != null)
                    isLoggedIn = true;
            }

            var likes = ArticleLikeService.GetAll()
                .Where(x => x.Article == article)
                .ToList();

            var comments = ArticleCommentService.GetArticleComments(article);

            var isLiked = false;

            if (isLoggedIn)
            {
                if (likes.FirstOrDefault(x => x.Author == user) != null)
                {
                    isLiked = true;
                }
            }

            var result = new ArticleViewModel(article, likes, comments, isLiked);
            result.Author.Rating = ReviewService.GetSpecialistRating(article.Author);

            return result;
        }

        private ArticleViewModel GetFullArticle(Article article)
        {
            var likes = ArticleLikeService.GetArticleLikes(article);
            var comments = ArticleCommentService.GetArticleComments(article);

            return new ArticleViewModel(article, likes, comments);
        }

        [HttpGet("customers/{userID}")]
        public IActionResult GetCustomer(long userID)
        {
            var user = UserService.Get(userID);
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problems = ProblemService.GetUserProblems(user);
            var sessions = SessionService.GetUserSessions(user)
                .Select(session => GetSpecialistSession(session))
                .ToList();

            return Ok(new DataResponse<SuperadminCustomerCard>
            {
                Data = new SuperadminCustomerCard
                {
                    UserID = user.ID,
                    FullName = $"{user.FirstName} {user.LastName}",
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    Sessions = sessions,
                    ProblemsCount = problems.Count,

                    SpendOrEarned = sessions
                        .Where(session => session.SessionStatus == SessionStatus.Success)
                        .ToList()
                        .Sum(session => session.Reward),

                    RefundsCount = sessions
                        .Where(session => session.SessionStatus == SessionStatus.Refund)
                        .ToList()
                        .Count
                }
            });
        }

        [HttpPut("customers/{userID}/role")]
        public IActionResult ChangeCustomerRole([FromBody] ChangeCustomerRoleRequest request, long userID)
        {
            var user = UserService.Get(userID);
            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            if (user.Role != request.Role)
            {
                if (request.Role == UserRole.Specialist)
                {
                    var specialist = SpecialistService.GetAllIncludesArchived()
                        .FirstOrDefault(x => x.User == user);

                    if (specialist != null)
                    {
                        if (specialist.Deleted)
                        {
                            specialist.Deleted = false;
                            SpecialistService.Update(specialist);

                            user.Role = UserRole.Specialist;
                            UserService.Update(user);

                            return Ok(new DataResponse<SuperadminCustomerCard>
                            {
                                Data = GetSuperadminCustomerCard(user)
                            });
                        }

                        return Ok(new DataResponse<SuperadminCustomerCard>
                        {
                            Data = GetSuperadminCustomerCard(user)
                        });
                    }

                    specialist = new Specialist { Price = 100, User = user };
                    SpecialistService.Create(specialist);

                    user.Role = UserRole.Specialist;
                    UserService.Update(user);

                    return Ok(new DataResponse<SuperadminCustomerCard>
                    {
                        Data = GetSuperadminCustomerCard(user)
                    });
                }

                if (request.Role == UserRole.Administrator)
                {
                    user.Role = UserRole.Administrator;
                    UserService.Update(user);

                    return Ok(new DataResponse<SuperadminCustomerCard>
                    {
                        Data = GetSuperadminCustomerCard(user)
                    });
                }

                if (request.Role == UserRole.Client)
                {
                    var specialist = SpecialistService.GetAllIncludesArchived()
                        .FirstOrDefault(x => x.User == user);

                    if (specialist != null && !specialist.Deleted)
                        SpecialistService.Delete(specialist);

                    user.Role = UserRole.Client;
                    UserService.Update(user);
                }

                return Ok(new DataResponse<SuperadminCustomerCard>
                {
                    Data = GetSuperadminCustomerCard(user)
                });
            }

            return Ok(new DataResponse<SuperadminCustomerCard>
            {
                Data = GetSuperadminCustomerCard(user)
            });
        }

        [HttpGet("customers/patients")]
        public IActionResult GetPatients([FromQuery] GetPatientsListRequest query)
        {
            var all = UserService.GetAll().Where(x => x.Role == UserRole.Client).ToList();
            var list = all.Select(x => GetSuperadminPatient(x)).ToList();

            if (query.SearchQuery != null && query.SearchQuery != "")
            {
                list = FilterListByQueryString(list, query.SearchQuery).ToHashSet().ToList();
            }
            
            var sortedList = SortPatientsList(list, query.SortBy, query.OrderBy);
            var pagination = PaginationHelper.PaginateEntityCollection(sortedList, query);

            return Ok(new GetPatientsListResponse
            {
                Data = pagination.Data,
                CurrentPage = pagination.CurrentPage,
                PageSize = pagination.PageSize,
                TotalPages = pagination.TotalPages,
                SortBy = query.SortBy,
                OrderBy = query.OrderBy,
                TotalItems = all.Count,
                SearchQuery = query.SearchQuery
            });
        }

        [HttpGet("customers/specialists")]
        public IActionResult GetSpecialists([FromQuery] GetSpecialistsListRequest query)
        {
            var all = UserService.GetAll().Where(x => x.Role == UserRole.Specialist).ToList();
            var list = all.Select(x => GetSuperadminSpecialist(x)).ToList();

            if (query.SearchQuery != null && query.SearchQuery != "")
            {
                list = FilterListByQueryString(list, query.SearchQuery).ToHashSet().ToList();
            }

            var sortedList = SortSpecialistList(list, query.SortBy, query.OrderBy);
            var pagination = PaginationHelper.PaginateEntityCollection(sortedList, query);
            
            return Ok(new GetSpecialistsListResponse
            {
                Data = pagination.Data,
                CurrentPage = pagination.CurrentPage,
                PageSize = pagination.PageSize,
                TotalPages = pagination.TotalPages,
                SortBy = query.SortBy,
                OrderBy = query.OrderBy,
                TotalItems = all.Count,
                SearchQuery = query.SearchQuery
            });
        }

        [HttpGet("customers/administrators")]
        public IActionResult GetAdministrators([FromQuery] GetAdministratorsListRequest query)
        {
            var all = UserService.GetAll().Where(x => x.Role == UserRole.Administrator).ToList();
            var list = all.Select(x => GetSuperadminAdministrator(x)).ToList();

            if (query.SearchQuery != null && query.SearchQuery != "")
            {
                list = FilterListByQueryString(list, query.SearchQuery).ToHashSet().ToList();
            }

            var sortedList = SortAdministratorsList(list, query.OrderBy);
            var pagination = PaginationHelper.PaginateEntityCollection(sortedList, query);

            return Ok(new GetAdministratorsListResponse
            {
                Data = pagination.Data,
                CurrentPage = pagination.CurrentPage,
                PageSize = pagination.PageSize,
                TotalPages = pagination.TotalPages,
                OrderBy = query.OrderBy,
                TotalItems = all.Count,
                SearchQuery = query.SearchQuery
            });
        }

        [HttpGet("reviews/video")]
        public IActionResult GetVideoReviews([FromQuery] GetList query)
        {
            var reviews = VideoReviewService.GetAll()
                .Select(x => new ClientVideoReviewViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<ClientVideoReviewViewModel>>
            {
                Data = reviews
            });
        }

        [HttpPost("reviews/video")]
        public IActionResult CreateVideoReview([FromBody] CreateVideoReviewRequest request)
        {
            if (request.Text.Length > 150)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Максимальная длина 150 символов"
                });

            var video = FileService.Get(request.VideoID);
            if (video == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Видео не найдено"
                });

            var review = new ClientVideoReview
            {
                FullName = request.FullName,
                Text = request.Text,
                LinkYouTube = video.Url,
                ReviewDate = DateTime.UtcNow
            };

            VideoReviewService.Create(review);

            return Ok(new DataResponse<ClientVideoReviewViewModel>
            {
                Data = new ClientVideoReviewViewModel(review)
            });
        }

        [HttpPut("reviews/video/{videoID}")]
        public IActionResult EditVideoReview([FromBody] CreateVideoReviewRequest request, long videoID)
        {
            var review = VideoReviewService.Get(videoID);
            if (review == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Отзыв не найден"
                });

            if (review.Text != request.Text)
            {
                if (request.Text.Length > 150)
                    return BadRequest(new ResponseModel
                    {
                        Success = false,
                        Message = "Максимальная длина 150 символов"
                    });

                review.Text = request.Text;
            }

            if (review.FullName != request.FullName)
                review.FullName = request.FullName;

            var video = FileService.Get(request.VideoID);
            if (video != null)
            {
                if (review.LinkYouTube != video.Url)
                    review.LinkYouTube = video.Url;
            }

            VideoReviewService.Update(review);

            return Ok(new DataResponse<ClientVideoReviewViewModel>
            {
                Data = new ClientVideoReviewViewModel(review)
            });
        }

        [HttpDelete("reviews/video/{videoID}")]
        public IActionResult DeleteVideoReview(long videoID)
        {
            var review = VideoReviewService.Get(videoID);
            if (review == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Отзыв не найден"
                });

            VideoReviewService.Delete(review);

            return Ok(new ResponseModel());
        }

        [HttpGet("articles/publish")]
        public IActionResult GetArticles([FromQuery] GetList query)
        {
            var all = ArticlePublishService.GetAll().ToList();
            var publishes = ArticlePublishService.GetAll()
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(publish => new ArticlePublishViewModel(publish, GetFullArticle(publish.Article)))
                .ToList();

            return Ok(new ListResponse<ArticlePublishViewModel>
            {
                Data = publishes,
                PageSize = query.PageSize,
                CurrentPage = query.PageNumber,
                TotalPages = (int)Math.Ceiling(all.Count / (double)query.PageSize),
                TotalItems = all.Count
            });
        }

        [HttpGet("articles/publish/{publishID}")]
        public IActionResult GetArticle(long publishID)
        {
            var publish = ArticlePublishService.Get(publishID);
            if (publish == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Статья не найдена"
                });

            return Ok(new DataResponse<ArticlePublishViewModel>
            {
                Data = new ArticlePublishViewModel(publish, GetFullArticle(publish.Article))
            });
        }

        [HttpPatch("articles/publish/{publishID}")]
        public IActionResult UpdateArticlePublish([FromBody] UpdateArticlePublishRequest request, long publishID)
        {
            var publish = ArticlePublishService.Get(publishID);
            if (publish == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Статья не найдена"
                });

            publish.Status = request.Status;
            publish.Message = request.Message;

            ArticlePublishService.Update(publish);

            publish.Article.ModerationStatus = request.Status;
            ArticleService.Update(publish.Article);

            return Ok(new DataResponse<ArticlePublishViewModel>
            {
                Data = new ArticlePublishViewModel(publish, GetFullArticle(publish.Article))
            });
        }

        [HttpGet("files/video")]
        public IActionResult GetVideoFiles([FromQuery] GetSearchListRequest query)
        {
            var all = FileService.GetAll()
                .Where(file =>
                    file.Type == FileType.MP4 ||
                    file.Type == FileType.MOV ||
                    file.Type == FileType.AVI)
                .OrderByDescending(file => file.ID)
                .ToList();

            var list = all.Select(file => new FileViewModel(file)).ToList();

            if (query.SearchQuery != null && query.SearchQuery != "")
            {
                list = FileService.FilterFilesByQueryString(list, query.SearchQuery).ToHashSet().ToList();
            }

            var pagination = PaginationHelper.PaginateEntityCollection(list, query);

            return Ok(new GetSearchListResponse
            {
                Data = pagination.Data,
                CurrentPage = pagination.CurrentPage,
                PageSize = pagination.PageSize,
                TotalPages = pagination.TotalPages,
                TotalItems = pagination.TotalItems,
                SearchQuery = query.SearchQuery
            });
        }

        [HttpGet("files/image")]
        public IActionResult GetImageFiles([FromQuery] GetSearchListRequest query)
        {
            var all = FileService.GetAll()
                .Where(file =>
                    file.Type == FileType.PNG ||
                    file.Type == FileType.JPEG ||
                    file.Type == FileType.GIF ||
                    file.Type == FileType.BMP)
                .OrderByDescending(file => file.ID)
                .ToList();

            var list = all.Select(file => new FileViewModel(file)).ToList();

            if (query.SearchQuery != null && query.SearchQuery != "")
            {
                list = FileService.FilterFilesByQueryString(list, query.SearchQuery).ToHashSet().ToList();
            }

            var pagination = PaginationHelper.PaginateEntityCollection(list, query);

            return Ok(new GetSearchListResponse
            {
                Data = pagination.Data,
                CurrentPage = pagination.CurrentPage,
                PageSize = pagination.PageSize,
                TotalPages = pagination.TotalPages,
                TotalItems = pagination.TotalItems,
                SearchQuery = query.SearchQuery
            });
        }

        [HttpDelete("files/{id}")]
        public IActionResult DeleteFile(long id)
        {
            var file = FileService.Get(id);
            if (file == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Файл не найден"
                });

            FileService.Delete(file);

            return Ok(new ResponseModel());
        }
    }
}
