using Microsoft.AspNetCore.Http;

namespace Domain.ViewModels.Request
{
    public class UploadFileFormRequest
    {
        public IFormFile File { get; set; }
    }
}
