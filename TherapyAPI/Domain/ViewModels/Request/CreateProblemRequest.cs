using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class CreateProblemRequest
    {
        [Required]
        public string ProblemText { get; set; }
    }
}
