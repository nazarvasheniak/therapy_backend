using System;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.Enums;
using Domain.Models;
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
            //string dir = System.IO.Path.Combine("/var", "www", "html", "files", now.Month.ToString());
            string dir = System.IO.Path.Combine("/Users", "user", "documents", "testfiles", now.Month.ToString());

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
                Url = $"http://185.43.5.164/files/{now.Month.ToString()}/{filename}"
            };

            Create(file);

            return file;
        }

        public FileType GetFileType(string base64string)
        {
            var strings = base64string.Split(",");

            switch (strings[0])
            {//check image's extension
                case "data:image/jpeg;base64":
                    return FileType.JPEG;

                case "data:image/png;base64":
                    return FileType.PNG;

                default:
                    return FileType.JPEG;
            }
        }

        public string GetFileTypeStr(string base64string)
        {
            var strings = base64string.Split(",");

            switch (strings[0])
            {//check image's extension
                case "data:image/jpeg;base64":
                    return "jpg";

                case "data:image/png;base64":
                    return "png";

                default:
                    return "jpg";
            }
        }
    }
}
