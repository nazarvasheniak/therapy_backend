using System;
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
    }
}
