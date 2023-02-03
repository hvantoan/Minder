using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Interfaces;
using Minder.Service.Models.File;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/file")]
    public class FileController : ControllerBase {
        private readonly IFileService fileService;

        public FileController(IFileService fileService) {
            this.fileService = fileService;
        }

        [HttpPost, Route("save")]
        public async Task<BaseResponse> Save(FileDto model) {
            try {
                await this.fileService.CreateOrUpdate(model);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
    }
}