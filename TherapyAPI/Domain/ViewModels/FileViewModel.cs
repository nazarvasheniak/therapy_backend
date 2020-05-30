using System;
using Domain.Enums;
using Domain.Models;

namespace Domain.ViewModels
{
    public class FileViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public FileType Type { get; set; }
        public string Url { get; set; }

        public FileViewModel(File file)
        {
            if (file != null)
            {
                ID = file.ID;
                Name = file.Name;
                Type = file.Type;
                Url = file.Url;
            }
        }
    }
}
