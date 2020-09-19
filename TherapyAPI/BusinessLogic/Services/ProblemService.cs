using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ProblemService : BaseCrudService<Problem>, IProblemService
    {
        public ProblemService(IRepository<Problem> repository) : base(repository)
        {
        }

        public int GetUserProblemsCount(User user)
        {
            return GetAll().Where(x => x.User == user).ToList().Count;
        }

        public List<Problem> GetUserProblems(User user)
        {
            return GetAll().Where(x => x.User == user).OrderByDescending(x => x.CreatedDate).ToList();
        }
    }
}
