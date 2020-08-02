using System;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Interfaces
{
    public interface IFileService : IBaseCrudService<File>
    {
        Task<File> SaveFile(string base64string);
        Task<File> SaveFileForm(IFormFile formFile);
        FileType GetFileType(string base64string);
        FileType GetFileType(IFormFile formFile);
        string GetFileTypeStr(string base64string);
        string GetFileTypeStr(IFormFile formFile);
    }
}
