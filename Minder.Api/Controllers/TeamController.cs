using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Database.Models;
using Minder.Service.Interfaces;
using Minder.Service.Models.Team;
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
        public async Task<BaseRes> Get(string teamId) {
            try {
                var data = await teamService.Get(teamId);
                return BaseRes<TeamDto?>.Ok(data);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("list")]
        public async Task<BaseRes> List(ListTeamReq req) {
            try {
                var data = await teamService.List(req);
                return BaseRes<ListTeamRes>.Ok(data);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("save")]
        public async Task<BaseRes> Save(TeamDto model) {
            try {
                var data = await teamService.CreateOrUpdate(model);
                return BaseSaveRes<Team>.Ok(data);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("delete/{teamId}")]
        public async Task<BaseRes> Delete(string teamId) {
            try {
                await teamService.Delete(teamId);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}