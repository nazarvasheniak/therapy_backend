using System;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IFileService : IBaseCrudService<File>
    {
        Task<File> SaveFile(string base64string);
        FileType GetFileType(string base64string);
        string GetFileTypeStr(string base64string);
    }
}
