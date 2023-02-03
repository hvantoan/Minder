using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Database.Models;
using Minder.Service.Interfaces;
using Minder.Service.Models;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/team")]
    public class TeamController : ControllerBase {
        private readonly ITeamService teamService;

        public TeamController(ITeamService teamService) {
            this.teamService = teamService;
        }

        [HttpGet, Route("get/{teamId}")]
        public async Task<BaseResponse> Get(string teamId) {
            try {
                var data = await teamService.Get(teamId);
                return BaseResponse<TeamDto?>.Ok(data);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("save")]
        public async Task<BaseResponse> Save(TeamDto model) {
            try {
                var data = await teamService.CreateOrUpdate(model);
                return BaseSaveData<Team>.Ok(data);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpGet, Route("delete/{teamId}")]
        public async Task<BaseResponse> Delete(string teamId) {
            try {
                await teamService.Delete(teamId);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
    }
}