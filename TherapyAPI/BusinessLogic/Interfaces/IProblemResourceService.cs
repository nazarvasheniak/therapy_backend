using System;
using System.Collections.Generic;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IProblemResourceService : IBaseCrudService<ProblemResource>
    {
        List<ProblemResource> GetProblemResources(Problem problem);
    }
}
