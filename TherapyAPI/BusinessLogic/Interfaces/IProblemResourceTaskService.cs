using System;
using System.Collections.Generic;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IProblemResourceTaskService : IBaseCrudService<ProblemResourceTask>
    {
        List<ProblemResourceTask> GetResourceTasks(ProblemResource resource);
    }
}
