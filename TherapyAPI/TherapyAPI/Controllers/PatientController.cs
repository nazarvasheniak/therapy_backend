using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.ViewModels;
using Domain.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TherapyAPI.Controllers
{
    [Route("api/patient")]
    [Authorize]
    public class PatientController : Controller
    {
        private IUserService UserService { get; set; }
        private IProblemService ProblemService { get; set; }
        private ISessionService SessionService { get; set; }

        public PatientController([FromServices]
            IUserService userService,
            IProblemService problemService,
            ISessionService sessionService)
        {
            UserService = userService;
            ProblemService = problemService;
            SessionService = sessionService;
        }

        [HttpGet("problems")]
        public IActionResult GetProblems()
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problems = ProblemService.GetAll().Where(x => x.User == user).Select(x => new ProblemViewModel(x)).ToList();

            return Ok(new DataResponse<List<ProblemViewModel>>
            {
                Data = problems
            });
        }

        [HttpGet("problems/{id}/sessions")]
        public IActionResult GetProblemSessions(long id)
        {
            var user = UserService.Get(long.Parse(User.Identity.Name));

            if (user == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Пользователь не найден"
                });

            var problem = ProblemService.Get(id);

            if (problem == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Проблема не найдена"
                });

            if (problem.User != user)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Доступ запрещен"
                });

            var sessions = SessionService.GetAll().Where(x => x.Problem == problem).OrderBy(x => x.Date).Select(x => new SessionViewModel(x)).ToList();

            return Ok(new DataResponse<List<SessionViewModel>>
            {
                Data = sessions
            });
        }
    }
}
