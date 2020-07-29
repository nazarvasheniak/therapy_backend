using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class CreateUpdateProblemResourceRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Emotion { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Characteristic { get; set; }

        [Required]
        public string Influence { get; set; }

        [Required]
        public int LikeScore { get; set; }

        public List<CreateUpdateProblemResourceTask> Tasks { get; set; }
    }
}
