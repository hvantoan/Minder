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

        [HttpPost, Route("save")]
        public async Task<BaseResponse> Save(TeamDto model) {
            try {
                var data = await teamService.CreateOrUpdate(model);
                return BaseSaveData<Team>.Ok(data);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
    }
}