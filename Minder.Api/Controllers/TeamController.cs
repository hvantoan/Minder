using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Database.Models;
using Minder.Service.Interfaces;
using Minder.Service.Models.Team;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/teams")]
    public class TeamController : ControllerBase {
        private readonly ITeamService teamService;

        public TeamController(ITeamService teamService) {
            this.teamService = teamService;
        }

        [HttpGet, Route("{teamId}")]
        public async Task<BaseRes> Get(string teamId) {
            try {
                var data = await teamService.Get(teamId);
                return BaseRes<TeamDto?>.Ok(data);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<BaseRes> List(string? searchText = null, int pageIndex = 0, int pageSize = 20, int count = 1, int isMyTeam = 0) {
            try {
                var req = new ListTeamReq() {
                    SearchText = searchText,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    IsCount = count == 1,
                    IsMyTeam = isMyTeam == 1
                };
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

        [HttpPost, Route("{teamId}/delete")]
        public async Task<BaseRes> Delete(string teamId) {
            try {
                await teamService.Delete(teamId);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("{id}/leave")]
        public async Task<BaseRes> Leave(string id) {
            try {
                await teamService.Leave(id);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("{userId}/kick")]
        public async Task<BaseRes> Kick(string userId) {
            try {
                await teamService.Kick(userId);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("find")]
        public async Task<BaseRes> Find(FindTeamReq req) {
            try {
                var res = await teamService.Find(req);
                return BaseRes<ListTeamRes?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}