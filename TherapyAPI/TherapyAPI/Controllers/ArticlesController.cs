using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    [Route("api/articles")]
    public class ArticlesController : Controller
    {
        private IArticleService ArticleService { get; set; }
        private IArticleLikeService ArticleLikeService { get; set; }
        private IArticleCommentService ArticleCommentService { get; set; }
        private IUserService UserService { get; set; }
        private ISpecialistService SpecialistService { get; set; }
        private IFileService FileService { get; set; }

        public ArticlesController([FromServices]
            IArticleService articleService,
            IArticleLikeService articleLikeService,
            IArticleCommentService articleCommentService,
            IUserService userService,
            ISpecialistService specialistService,
            IFileService fileService)
        {
            ArticleService = articleService;
            ArticleLikeService = articleLikeService;
            ArticleCommentService = articleCommentService;
            UserService = userService;
            SpecialistService = specialistService;
            FileService = fileService;
        }

        private ArticleViewModel GetFullArticle(long id)
        {
            var article = ArticleService.Get(id);

            if (article == null)
                return null;

            var isLoggedIn = false;

            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user != null)
                isLoggedIn = true;

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

            return new ArticleViewModel(article, likes, comments, isLiked);
        }

        private List<ArticleViewModel> GetFullArticles(GetList query)
        {
            var isLoggedIn = false;

            var user = UserService.Get(long.Parse(User.Identity.Name));
            if (user != null)
                isLoggedIn = true;

            var result = new List<ArticleViewModel>();

            var articles = ArticleService.GetAll()
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

                result.Add(new ArticleViewModel(article, likes, comments, isLiked));
            });

            return result;
        }

        [HttpGet]
        public IActionResult GetArticles([FromQuery] GetList query)
        {
            var all = ArticleService.GetAll().ToList();

            var articles = GetFullArticles(query);

            return Ok(new ListResponse<ArticleViewModel>
            {
                Data = articles,
                PageSize = query.PageSize,
                CurrentPage = query.PageNumber,
                TotalPages = (int)Math.Ceiling(all.Count / (double)query.PageSize)
            });
        }

        [HttpGet("my")]
        [Authorize]
        public IActionResult GetMyArticles()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            if (user.Role == UserRole.Client)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var specialist = SpecialistService.GetSpecialistFromUser(user);

            if (specialist == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var articles = ArticleService.GetAll()
                .Where(x => x.Author == specialist)
                .Select(x => new ArticleViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<ArticleViewModel>>
            {
                Data = articles
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetArticle(long id)
        {
            var article = ArticleService.Get(id);

            if (article == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Статья не найдена"
                });

            return Ok(new DataResponse<ArticleViewModel>
            {
                Data = new ArticleViewModel(article)
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateArticle([FromBody] CreateUpdateArticleRequest request)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            if (user.Role == UserRole.Client)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            if (ArticleService.IsArticleExist(request.Title))
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Статья с таким названием уже существует"
                });

            var specialist = SpecialistService.GetSpecialistFromUser(user);

            if (specialist == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var previewImage = FileService.Get(request.PreviewImageID);

            if (previewImage == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Изображение не найдено"
                });

            var article = new Article
            {
                Title = request.Title,
                ShortText = request.ShortText,
                Text = Encoding.UTF8.GetBytes(request.Text),
                Image = previewImage,
                Author = specialist,
                Date = DateTime.UtcNow
            };

            ArticleService.Create(article);

            return Ok(new DataResponse<ArticleViewModel>
            {
                Data = new ArticleViewModel(article)
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateArticle([FromBody] CreateUpdateArticleRequest request, long id)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            if (user.Role == UserRole.Client)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var specialist = SpecialistService.GetSpecialistFromUser(user);

            if (specialist == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var article = ArticleService.Get(id);

            if (article == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Статья не найдена"
                });

            if (article.Author != specialist)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Ошибка доступа"
                });

            var previewImage = FileService.Get(request.PreviewImageID);

            if (previewImage == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Изображение не найдено"
                });

            // TODO Обновлять дату при редактировании?
            article.Title = request.Title;
            article.ShortText = request.ShortText;
            article.Text = Encoding.UTF8.GetBytes(request.Text);
            article.Image = previewImage;

            ArticleService.Update(article);

            return Ok(new DataResponse<ArticleViewModel>
            {
                Data = new ArticleViewModel(article)
            });
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public IActionResult LikeArticle(long id)
        {
            var article = ArticleService.Get(id);

            if (article == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Статья не найдена"
                });

            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var like = ArticleLikeService.GetAll()
                .FirstOrDefault(x => x.Article == article && x.Author == user);

            if (like != null)
                ArticleLikeService.Delete(like);
            else
                ArticleLikeService.Create(like = new ArticleLike
                {
                    Article = article,
                    Author = user
                });

            return Ok(new ResponseModel());
        }

        [HttpPost("{id}/comment")]
        [Authorize]
        public IActionResult CommentArticle([FromBody] CreateArticleCommentRequest request, long id)
        {
            var article = ArticleService.Get(id);

            if (article == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Статья не найдена"
                });

            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            ArticleComment parentComment = null;

            if (request.IsReply && request.ParentCommentID != 0)
            {
                parentComment = ArticleCommentService.Get(request.ParentCommentID);

                if (parentComment == null)
                    return NotFound(new ResponseModel
                    {
                        Success = false,
                        Message = "Комментарий не найден"
                    });
            }


            var comment = new ArticleComment
            {
                Article = article,
                Author = user,
                IsReply = request.IsReply,
                ParentComment = parentComment,
                Text = request.Text
            };

            ArticleCommentService.Create(comment);

            return Ok(new DataResponse<ArticleCommentViewModel>
            {
                Data = new ArticleCommentViewModel(comment)
            });
        }
    }
}
