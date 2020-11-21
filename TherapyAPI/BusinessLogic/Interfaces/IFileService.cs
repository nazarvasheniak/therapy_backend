using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Interfaces
{
    public interface IFileService : IBaseCrudService<File>
    {
        Task<File> SaveFile(string base64string);
        Task<File> SaveFileForm(IFormFile formFile);
        File GetFileByUrl(string url);
        IEnumerable<FileViewModel> FilterFilesByQueryString(List<FileViewModel> list, string query);
        FileType GetFileType(string base64string);
        FileType GetFileType(IFormFile formFile);
        string GetFileTypeStr(string base64string);
        string GetFileTypeStr(IFormFile formFile);
    }
}
