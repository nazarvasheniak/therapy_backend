using System;
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
    }
}
