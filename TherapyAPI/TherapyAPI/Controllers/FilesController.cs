using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Domain.ViewModels;
using Domain.ViewModels.Request;
using Domain.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TherapyAPI.Controllers
{
    [Route("api/files")]
    public class FilesController : Controller
    {
        private IFileService FileService { get; set; }

        public FilesController([FromServices]
            IFileService fileService)
        {
            FileService = fileService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromBody] UploadFileRequest request)
        {
            var file = await FileService.SaveFile(request.Base64string);
            if (file == null)
                return BadRequest(new ResponseModel
                {
                    Success = false,
                    Message = "Поле Файл не может быть пустым"
                });

            return Ok(new DataResponse<FileViewModel>
            {
                Data = new FileViewModel(file)
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetFile(long id)
        {
            var file = FileService.Get(id);

            if (file == null)
                return NotFound(new ResponseModel
                {
                    Success = false,
                    Message = "Файл не найден"
                });

            return Ok(new DataResponse<FileViewModel>
            {
                Data = new FileViewModel(file)
            });
        }
    }
}
