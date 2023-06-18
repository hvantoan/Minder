using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Database.Enums;
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

        [HttpPost, Route("list/{id}")]
        public async Task<BaseRes> ListById([FromRoute] string id, [FromQuery] EItemType type = EItemType.GroupImage) {
            try {
                var res = await fileService.ListById(id, type);
                return BaseRes<ListFileRes>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("update")]
        public async Task<BaseRes> Update(FileDto model) {
            try {
                await this.fileService.Update(model);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("create")]
        public async Task<BaseRes> Create(CreateImageReq req) {
            try {
                var model = new FileDto {
                    ItemId = req.ItemId,
                    Data = req.Data,
                    ItemType = req.ItemType,
                    Type = EFile.Image,
                    Name = req.FileName,
                    ImportUrl = req.ImportUrl,
                };

                await this.fileService.Create(model);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}