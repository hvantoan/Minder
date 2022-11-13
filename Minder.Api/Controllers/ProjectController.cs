using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ToolSeoViet.Service.Interfaces;
using ToolSeoViet.Service.Models.Project;
using ToolSeoViet.Services.Models;

namespace ToolSeoViet.Api.Controllers {
    [ApiController, Authorize, Route("api/project")]
    public class ProjectController : ControllerBase {
        private readonly IProjectService projectService;

        public ProjectController(IProjectService projectService) {
            this.projectService = projectService;
        }

        [HttpGet, Route("all")]
        public async Task<BaseResponse> All() {
            try {
                var response = await this.projectService.All();
                return BaseResponse<ListProjectResponse>.Ok(response);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
        [HttpPost, Route("get")]
        public async Task<BaseResponse> Get(GetProjectRequest request) {
            try {
                var response = await this.projectService.Get(request.Id);
                return BaseResponse<ProjectDto>.Ok(response);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("save")]
        public async Task<BaseResponse> Save(SaveProjectRequest request) {
            try {
                await this.projectService.CreateOrUpdate(request);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("delete")]
        public async Task<BaseResponse> Delete(DeleteProjectRequest request) {
            try {
                await this.projectService.Delete(request.Id);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
    }
}
