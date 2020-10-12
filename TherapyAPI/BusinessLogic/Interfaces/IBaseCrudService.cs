using System;
using System.Linq;

namespace BusinessLogic.Interfaces
{
    public interface IBaseCrudService<T>
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetAllIncludesArchived();
        T Get(long id);
        void Create(T item);
        void Update(T item);
        void Delete(T item);
        void Delete(long id);
    }
}
