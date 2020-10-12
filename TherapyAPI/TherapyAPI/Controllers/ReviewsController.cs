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
    [Route("api/reviews")]
    public class ReviewsController : Controller
    {
        private IUserService UserService { get; set; }
        private IReviewService ReviewService { get; set; }
        private IClientVideoReviewService VideoReviewService { get; set; }

        public ReviewsController([FromServices]
            IUserService userService,
            IReviewService reviewService,
            IClientVideoReviewService videoReviewService)
        {
            UserService = userService;
            ReviewService = reviewService;
            VideoReviewService = videoReviewService;
        }

        [HttpGet("video")]
        public IActionResult GetAllVideoReviews()
        {
            var reviews = VideoReviewService.GetAll()
                .Select(x => new ClientVideoReviewViewModel(x))
                .ToList();

            return Ok(new DataResponse<List<ClientVideoReviewViewModel>>
            {
                Data = reviews
            });
        }
    }
}
