using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ProblemResourceService : BaseCrudService<ProblemResource>, IProblemResourceService
    {
        public ProblemResourceService(IRepository<ProblemResource> repository) : base(repository)
        {
        }

        public List<ProblemResource> GetProblemResources(Problem problem)
        {
            return GetAll().Where(x => x.Session.Problem == problem).OrderByDescending(x => x.Session.Date).ToList();
        }
    }
}
