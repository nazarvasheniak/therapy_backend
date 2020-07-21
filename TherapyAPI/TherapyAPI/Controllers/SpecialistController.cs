using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.Models;
using Domain.ViewModels;
using Domain.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public SpecialistController([FromServices]
            IUserService userService,
            ISpecialistService specialistService,
            IReviewService reviewService)
        {
            UserService = userService;
            SpecialistService = specialistService;
            ReviewService = reviewService;
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
    }
}
