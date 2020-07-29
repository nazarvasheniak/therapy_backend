using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class CreateProblemResourceTask
    {
        [Required]
        public string Title { get; set; }
    }
}
