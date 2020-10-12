using System;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface ISpecialistService : IBaseCrudService<Specialist>
    {
        Specialist GetSpecialistFromUser(User user);
        Specialist CreateSpecialistFromUser(User user);
    }
}
