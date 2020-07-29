using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class CreateUpdateProblemResourceTask
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public bool IsDone { get; set; }
    }
}
