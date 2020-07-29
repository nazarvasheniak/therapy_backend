using System;
using System.Collections.Generic;
using Domain.Models;
using Domain.ViewModels;
using Domain.ViewModels.Request;

namespace BusinessLogic.Interfaces
{
    public interface IProblemResourceTaskService : IBaseCrudService<ProblemResourceTask>
    {
        List<ProblemResourceTask> GetResourceTasks(ProblemResource resource);
        bool IsTaskExist(string name, ProblemResource resource);
        ProblemResourceTask CreateTask(CreateUpdateProblemResourceTask request, ProblemResource resource);
        ProblemResourceTask UpdateTask(CreateUpdateProblemResourceTask request);
        ProblemResourceTask CreateUpdateTask(CreateUpdateProblemResourceTask request, ProblemResource resource = null);
    }
}
