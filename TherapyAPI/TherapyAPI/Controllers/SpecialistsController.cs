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

        private List<SpecialistViewModel> GetFullSpecialists(GetList query)
        {
            var result = new List<SpecialistViewModel>();

            var specialists = SpecialistService.GetAll()
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            specialists.ForEach(specialist => result.Add(GetFullSpecialist(specialist)));

            return result;
        }

        [HttpGet]
        public IActionResult GetSpecialists([FromQuery] GetList query)
        {
            var all = SpecialistService.GetAll().ToList();
            var specialists = GetFullSpecialists(query);

            return Ok(new ListResponse<SpecialistViewModel>
            {
                Data = specialists,
                PageSize = query.PageSize,
                CurrentPage = query.PageNumber,
                TotalPages = (int)Math.Ceiling(all.Count / (double)query.PageSize)
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetSpecialist(long id)
        {
            var specialist = GetFullSpecialist(id);

            if (specialist == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Специалист не найден"
                });

            return Ok(new DataResponse<SpecialistViewModel>
            {
                Data = specialist
            });
        }

        [HttpGet("{id}/reviews")]
        public IActionResult GetSpecialistReviews([FromQuery] GetReviews query, long id)
        {
            var specialist = SpecialistService.Get(id);

            if (specialist == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Специалист не найден"
                });

            var reviews = ReviewService.GetSpecialistReviews(specialist, query.Type);

            return Ok(new ListResponse<ReviewViewModel>
            {
                Data = reviews.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToList(),
                PageSize = query.PageSize,
                CurrentPage = query.PageNumber,
                TotalPages = (int)Math.Ceiling(reviews.Count / (double)query.PageSize)
            });                    
        }
    }
}
