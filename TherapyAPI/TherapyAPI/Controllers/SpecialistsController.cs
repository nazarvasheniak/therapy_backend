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
    [Route("api/specialists")]
    public class SpecialistsController : Controller
    {
        private ISpecialistService SpecialistService { get; set; }
        private IReviewService ReviewService { get; set; }

        public SpecialistsController([FromServices]
            ISpecialistService specialistService,
            IReviewService reviewService)
        {
            SpecialistService = specialistService;
            ReviewService = reviewService;
        }

        [HttpGet]
        public IActionResult GetSpecialists()
        {
            var specialists = SpecialistService.GetAll().Select(x => new SpecialistViewModel(x)).ToList();

            return Ok(new DataResponse<List<SpecialistViewModel>>
            {
                Data = specialists
            });
        }

        [HttpGet("{id}/reviews")]
        public IActionResult GetSpecialistReviews(long id)
        {
            var specialist = SpecialistService.Get(id);

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

            return Ok(new DataResponse<List<ReviewViewModel>>
            {
                Data = reviews
            });
        }
    }
}
