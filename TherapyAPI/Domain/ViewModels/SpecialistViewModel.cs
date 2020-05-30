using System;
using System.Collections.Generic;
using Domain.Models;

namespace Domain.ViewModels
{
    public class SpecialistViewModel
    {
        public long ID { get; set; }
        public UserViewModel User { get; set; }
        public FileViewModel Photo { get; set; }
        public double Price { get; set; }

        public SpecialistViewModel(Specialist specialist)
        {
            if (specialist != null)
            {
                ID = specialist.ID;
                User = new UserViewModel(specialist.User);
                Photo = new FileViewModel(specialist.Photo);
                Price = specialist.Price;
            }
        }
    }
}
