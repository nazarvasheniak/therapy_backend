using System;
using System.Collections.Generic;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IProblemImageService : IBaseCrudService<ProblemImage>
    {
        List<ProblemImage> GetProblemImages(Problem problem);
    }
}
