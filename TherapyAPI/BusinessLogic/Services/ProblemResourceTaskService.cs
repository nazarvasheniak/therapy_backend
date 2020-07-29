using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ProblemResourceTaskService : BaseCrudService<ProblemResourceTask>, IProblemResourceTaskService
    {
        public ProblemResourceTaskService(IRepository<ProblemResourceTask> repository) : base(repository)
        {
        }

        public List<ProblemResourceTask> GetResourceTasks(ProblemResource resource)
        {
            return GetAll().Where(x => x.Resource == resource).ToList();
        }
    }
}
