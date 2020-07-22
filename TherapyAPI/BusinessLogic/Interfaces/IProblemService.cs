using System;
using System.Collections.Generic;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IProblemService : IBaseCrudService<Problem>
    {
        int GetUserProblemsCount(User user);
        List<Problem> GetUserProblems(User user);
    }
}
