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
        private ISessionService SessionService { get; set; }
        private IReviewService ReviewService { get; set; }

        public SpecialistsController([FromServices]
            ISpecialistService specialistService,
            ISessionService  sessionService,
            IReviewService reviewService)
        {
            SpecialistService = specialistService;
            SessionService = sessionService;
            ReviewService = reviewService;
        }

        private SpecialistViewModel GetFullSpecialist(long id)
        {
            var specialist = SpecialistService.Get(id);

            if (specialist == null)
                return null;

            var reviews = ReviewService.GetAll()
                .Where(x => x.Session.Specialist == specialist)
                .OrderByDescending(x => x.Session.Date)
                .Select(x => new ReviewViewModel(x))
                .ToList();

            var rating = ReviewService.GetSpecialistRating(specialist);

            return new SpecialistViewModel(specialist, rating, reviews);
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

        private List<SpecialistViewModel> GetFullSpecialists(GetSpecialistsList query)
        {
            var result = new List<SpecialistViewModel>();
            var specialists = new List<Specialist>();

            if (query.OrderBy == OrderBy.ASC)
            {
                switch (query.SortBy)
                {
                    case SpecialistsSort.Price:
                        {
                            specialists.AddRange(SpecialistService.GetAll().ToList()
                                .OrderBy(x => x.Price)
                                .Skip((query.PageNumber - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToList());

                            break;
                        }

                    case SpecialistsSort.Rating:
                        {
                            specialists.AddRange(SpecialistService.GetAll().ToList()
                                .OrderBy(x => ReviewService.GetSpecialistRating(x))
                                .Skip((query.PageNumber - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToList());

                            break;
                        }

                    case SpecialistsSort.Reviews:
                        {
                            specialists.AddRange(SpecialistService.GetAll().ToList()
                                .OrderBy(x => SessionService.GetSpecialistSessions(x).Count)
                                .Skip((query.PageNumber - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToList());

                            break;
                        }

                    default:
                        {
                            specialists.AddRange(SpecialistService.GetAll().ToList()
                                .OrderBy(x => x.Price)
                                .Skip((query.PageNumber - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToList());

                            break;
                        }
                }
            }
            else
            {
                switch (query.SortBy)
                {
                    case SpecialistsSort.Price:
                        {
                            specialists.AddRange(SpecialistService.GetAll().ToList()
                                .OrderByDescending(x => x.Price)
                                .Skip((query.PageNumber - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToList());

                            break;
                        }

                    case SpecialistsSort.Rating:
                        {
                            specialists.AddRange(SpecialistService.GetAll().ToList()
                                .OrderByDescending(x => ReviewService.GetSpecialistRating(x))
                                .Skip((query.PageNumber - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToList());

                            break;
                        }

                    case SpecialistsSort.Reviews:
                        {
                            specialists.AddRange(SpecialistService.GetAll().ToList()
                                .OrderByDescending(x => SessionService.GetSpecialistSessions(x).Count)
                                .Skip((query.PageNumber - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToList());

                            break;
                        }

                    default:
                        {
                            specialists.AddRange(SpecialistService.GetAll().ToList()
                                .OrderByDescending(x => x.Price)
                                .Skip((query.PageNumber - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToList());

                            break;
                        }
                }
            }

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
                TotalPages = (int)Math.Ceiling(all.Count / (double)query.PageSize),
                TotalItems = all.Count
            });
        }

        [HttpGet("sorted")]
        public IActionResult GetSpecialists([FromQuery] GetSpecialistsList query)
        {
            var all = SpecialistService.GetAll().ToList();
            var specialists = GetFullSpecialists(query);

            return Ok(new SpecialistsListResponse
            {
                Data = specialists,
                PageSize = query.PageSize,
                CurrentPage = query.PageNumber,
                SortBy = query.SortBy,
                OrderBy = query.OrderBy,
                TotalPages = (int)Math.Ceiling(all.Count / (double)query.PageSize),
                TotalItems = all.Count
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
                TotalPages = (int)Math.Ceiling(reviews.Count / (double)query.PageSize),
                TotalItems = reviews.Count
            });                    
        }
    }
}
