using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swync.Core.Onedrive;

namespace Swync.Api.Controllers.Api.Onedrive
{
    [Route("api/onedrive/[controller]")]
    public class FilesController : Controller
    {
        private readonly OnedriveService _service;

        public FilesController(OnedriveService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public IEnumerable<OnedriveFile> Get()
        {
            return _service.GetFiles();
        }
    }
}