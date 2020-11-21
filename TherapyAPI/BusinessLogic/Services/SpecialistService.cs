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

        public Specialist CreateSpecialistFromUser(User user)
        {
            var specialist = GetAllIncludesArchived().FirstOrDefault(x => x.User == user);
            if (specialist != null)
                return specialist;

            specialist = new Specialist { User = user, Price = 1000 };
            Create(specialist);

            return specialist;
        }
    }
}
