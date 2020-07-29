using System;
using System.Collections.Generic;

namespace Domain.ViewModels
{
    public class ProblemAssetsViewModel
    {
        public ProblemViewModel Problem { get; set; }
        public List<ProblemImageViewModel> Images { get; set; }
        public List<ProblemResourceViewModel> Resources { get; set; }
        public List<SpecialistSessionViewModel> Sessions { get; set; }
    }
}
