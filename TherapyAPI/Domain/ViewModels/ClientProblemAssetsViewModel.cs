using System;
using System.Collections.Generic;

namespace Domain.ViewModels
{
    public class ClientProblemAssetsViewModel
    {
        public ProblemViewModel Problem { get; set; }
        public List<ProblemImageViewModel> Images { get; set; }
        public List<ProblemResourceViewModel> Resources { get; set; }
        public List<SessionViewModel> Sessions { get; set; }
    }
}
