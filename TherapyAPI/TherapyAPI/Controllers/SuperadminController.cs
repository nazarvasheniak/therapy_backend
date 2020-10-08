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

        public SuperadminController([FromServices]
            IUserService userService,
            ISpecialistService specialistService,
            ISessionService sessionService,
            IReviewService reviewService
        )
        {
            UserService = userService;
            SpecialistService = specialistService;
            SessionService = sessionService;
            ReviewService = reviewService;
        }

        private SuperadminPatientModel GetSuperadminPatient(User user)
        {
            var result = new SuperadminPatientModel
            {
                User = new UserViewModel(user),
                AverageScore = ReviewService.GetUserAverageScore(user)
            };

            var sessions = SessionService.GetUserSessions(user)
                .Where(x => x.Status == SessionStatus.Success || x.Status == SessionStatus.Refund)
                .ToList();

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
    }
}
