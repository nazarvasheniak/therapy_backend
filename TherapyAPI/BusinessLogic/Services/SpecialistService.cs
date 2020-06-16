using System;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class SpecialistService : BaseCrudService<Specialist>, ISpecialistService
    {
        public SpecialistService(IRepository<Specialist> repository) : base(repository)
        {
        }

        public Specialist GetSpecialistFromUser(User user)
        {
            return GetAll().FirstOrDefault(x => x.User == user);
        }
    }
}
