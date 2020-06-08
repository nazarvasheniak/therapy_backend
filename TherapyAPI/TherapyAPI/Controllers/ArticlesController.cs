using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.ViewModels;
using Domain.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TherapyAPI.Controllers
{
    [Route("api/articles")]
    public class ArticlesController : Controller
    {
        private IArticleService ArticleService { get; set; }

        public ArticlesController([FromServices]
            IArticleService articleService)
        {
            ArticleService = articleService;
        }

        [HttpGet]
        public IActionResult GetArticles()
        {
            var articles = ArticleService.GetAll().Select(x => new ArticleViewModel(x)).ToList();

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
    }
}
