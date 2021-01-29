using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Config;
using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class FileService : BaseCrudService<File>, IFileService
    {
        private readonly string UploadsDir = System.IO.Path.Combine("/var", "www", "www-root", "data", "www", "static.kornevaya.ru", "uploads");
        //private readonly string UploadsDir = System.IO.Path.Combine("/Users", "user", "documents", "testfiles");
        //string private readonly dir = System.IO.Path.Combine("C:", "OpenServer", "domains", "files");

        public FileService(IRepository<File> repository) : base(repository)
        {
        }

        public async Task<File> SaveFile(string base64string)
        {
            var now = DateTime.UtcNow;
            var dir = System.IO.Path.Combine(UploadsDir, now.Month.ToString());

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
            var now = DateTime.UtcNow;
            var dir = System.IO.Path.Combine(UploadsDir, now.Month.ToString());

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

        public File GetFileByUrl(string url)
        {
            return GetAll().FirstOrDefault(file => file.Url == url);
        }

        public IEnumerable<FileViewModel> FilterFilesByQueryString(List<FileViewModel> list, string query)
        {
            foreach (var item in list)
            {
                if (item.Name.ToLower().Contains(query))
                    yield return item;

                if (item.Type.ToString().ToLower().Contains(query))
                    yield return item;

                if (item.Url.ToLower().Contains(query))
                    yield return item;
            }
        }

        public FileType GetFileType(string base64string)
        {
            var strings = base64string.Split(",");

            return (strings[0]) switch
            {
                "data:image/jpeg;base64" => FileType.JPEG,
                "data:image/jpg;base64" => FileType.JPEG,
                "data:image/png;base64" => FileType.PNG,
                "data:image/svg+xml;base64" => FileType.SVG,
                "data:video/mp4;base64" => FileType.MP4,
                "data:video/quicktime;base64" => FileType.MOV,
                _ => FileType.TXT,
            };
        }

        public FileType GetFileType(IFormFile formFile)
        {
            return formFile.ContentType switch
            {
                "image/png" => FileType.PNG,
                "image/jpeg" => FileType.JPEG,
                "image/jpg" => FileType.JPEG,
                "image/svg+xml" => FileType.SVG,
                "video/mp4" => FileType.MP4,
                "video/quicktime" => FileType.MOV,
                _ => FileType.TXT,
            };
        }

        public string GetFileTypeStr(string base64string)
        {
            var strings = base64string.Split(",");

            return (strings[0]) switch
            {
                "data:image/jpeg;base64" => "jpg",
                "data:image/jpg;base64" => "jpg",
                "data:image/png;base64" => "png",
                "data:image/svg+xml;base64" => "svg",
                "data:video/mp4;base64" => "mp4",
                "data:video/quicktime;base64" => "mov",
                _ => "txt",
            };
        }

        public string GetFileTypeStr(IFormFile formFile)
        {
            return formFile.ContentType switch
            {
                "image/png" => "png",
                "image/jpeg" => "jpg",
                "image/jpg" => "jpg",
                "image/svg+xml" => "svg",
                "video/mp4" => "mp4",
                "video/quicktime" => "mov",
                _ => "txt",
            };
        }
    }
}
