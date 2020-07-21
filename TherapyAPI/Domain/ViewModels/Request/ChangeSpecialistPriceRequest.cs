using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels.Request
{
    public class ChangeSpecialistPriceRequest
    {
        [Required]
        public double Price { get; set; }
    }
}
