using System;
using BusinessLogic.Interfaces;
using Domain.Models;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class FileService : BaseCrudService<File>, IFileService
    {
        public FileService(IRepository<File> repository) : base(repository)
        {
        }
    }
}
