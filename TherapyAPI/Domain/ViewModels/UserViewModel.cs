using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Domain.Enums;
using Domain.Models;

namespace Domain.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public FileViewModel Photo { get; set; }
        public UserRole Role { get; set; }
        public DateTime RegisteredAt { get; set; }

        public UserViewModel(User user)
        {
            if (user != null)
            {
                ID = user.ID;
                FirstName = user.FirstName;
                LastName = user.LastName;
                PhoneNumber = user.PhoneNumber;
                Email = user.Email;
                Photo = new FileViewModel(user.Photo);
                Role = user.Role;
                RegisteredAt = user.RegisteredAt;
            }
        }
    }
}
