using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class SetProblemSpecialistRequest
    {
        [Required]
        public long SpecialistID { get; set; }
    }
}
