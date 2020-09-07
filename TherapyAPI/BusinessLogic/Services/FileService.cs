using System;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Config;
using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class FileService : BaseCrudService<File>, IFileService
    {
        public FileService(IRepository<File> repository) : base(repository)
        {
        }

        public async Task<File> SaveFile(string base64string)
        {
            var now = DateTime.Now;
            string dir = System.IO.Path.Combine("/var", "www", "www-root", "data", "www", "static.kornevaya.ru", "uploads", now.Month.ToString());
            //string dir = System.IO.Path.Combine("/Users", "user", "documents", "testfiles", now.Month.ToString());
            //string dir = System.IO.Path.Combine("C:", "OpenServer", "domains", "files", now.Month.ToString());

            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            
            string filename = $"{(now.Hour.ToString() + now.Minute.ToString() + now.Second.ToString() + now.Millisecond.ToString()).Replace(" ", "").Replace(".", "").Replace(":", "")}.{GetFileTypeStr(base64string)}";
            string path = System.IO.Path.Combine(dir, filename);
            if (base64string.Length <= 0)
                return null;

            System.IO.File.Create(path).Close();
            System.IO.File.WriteAllBytes(path, Convert.FromBase64String(base64string.Split(",")[1]));


            var file = new File
            {
                Name = filename,
                Type = GetFileType(base64string),
                LocalPath = path,
                Url = $"{AppSettings.StaticWebUrl}/uploads/{now.Month}/{filename}"
            };

            Create(file);

            return file;
        }

        public async Task<File> SaveFileForm(IFormFile formFile)
        {
            var now = DateTime.Now;
            string dir = System.IO.Path.Combine("/var", "www", "www-root", "data", "www", "static.kornevaya.ru", "uploads", now.Month.ToString());
            //string dir = System.IO.Path.Combine("/Users", "user", "documents", "testfiles", now.Month.ToString());
            //string dir = System.IO.Path.Combine("C:", "OpenServer", "domains", "files", now.Month.ToString());

            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            string filename = $"{(now.Hour.ToString() + now.Minute.ToString() + now.Second.ToString() + now.Millisecond.ToString()).Replace(" ", "").Replace(".", "").Replace(":", "")}.{GetFileTypeStr(formFile)}";
            string path = System.IO.Path.Combine(dir, filename);

            using var stream = System.IO.File.Create(path);
            await formFile.CopyToAsync(stream);
            stream.Close();

            var dbRecord = new File
            {
                Name = formFile.FileName,
                Type = GetFileType(formFile),
                LocalPath = path,
                Url = $"{AppSettings.StaticWebUrl}/uploads/{now.Month}/{filename}"
            };

            Create(dbRecord);

            return dbRecord;
        }

        public FileType GetFileType(string base64string)
        {
            var strings = base64string.Split(",");

            return (strings[0]) switch
            {
                "data:image/jpeg;base64" => FileType.JPEG,
                "data:image/png;base64" => FileType.PNG,
                _ => FileType.JPEG,
            };
        }

        public FileType GetFileType(IFormFile formFile)
        {
            return formFile.ContentType switch
            {
                "image/png" => FileType.PNG,
                "image/jpeg" => FileType.JPEG,
                "image/jpg" => FileType.JPEG,
                _ => FileType.PNG,
            };
        }

        public string GetFileTypeStr(string base64string)
        {
            var strings = base64string.Split(",");

            return (strings[0]) switch
            {
                "data:image/jpeg;base64" => "jpg",
                "data:image/png;base64" => "png",
                _ => "jpg",
            };
        }

        public string GetFileTypeStr(IFormFile formFile)
        {
            return formFile.ContentType switch
            {
                "image/png" => "png",
                "image/jpeg" => "jpg",
                "image/jpg" => "jpg",
                _ => "png",
            };
        }
    }
}
