using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ProblemImageService : BaseCrudService<ProblemImage>, IProblemImageService
    {
        public ProblemImageService(IRepository<ProblemImage> repository) : base(repository)
        {
        }

        public List<ProblemImage> GetProblemImages(Problem problem)
        {
            return GetAll().Where(x => x.Session.Problem == problem).OrderByDescending(x => x.Session.Date).ToList();
        }
    }
}
