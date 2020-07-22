using System;
using System.Collections.Generic;
using Domain.Models;

namespace Domain.ViewModels
{
    public class SpecialistViewModel : BaseViewModel
    {
        public UserViewModel User { get; set; }
        public FileViewModel Photo { get; set; }
        public double Price { get; set; }
        public double Rating { get; set; }
        public List<ReviewViewModel> Reviews { get; set; }

        public SpecialistViewModel(Specialist specialist)
        {
            if (specialist != null)
            {
                ID = specialist.ID;
                User = new UserViewModel(specialist.User);
                Photo = new FileViewModel(specialist.Photo);
                Price = specialist.Price;
                Rating = 0;
                Reviews = new List<ReviewViewModel>();
            }
        }

        public SpecialistViewModel(Specialist specialist, double rating, List<ReviewViewModel> reviews)
        {
            if (specialist != null)
            {
                ID = specialist.ID;
                User = new UserViewModel(specialist.User);
                Photo = new FileViewModel(specialist.Photo);
                Price = specialist.Price;
                Rating = rating;
                Reviews = reviews;
            }
        }
    }
}
