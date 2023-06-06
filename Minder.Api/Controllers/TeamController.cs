using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Interfaces;
using Minder.Service.Models.Team;
using Minder.Service.Models.User;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;
using static Minder.Service.Enums;

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
        public async Task<BaseRes> Save(SaveTeamRequest model) {
            try {
                var data = await teamService.CreateOrUpdate(model);
                return BaseRes<TeamDto?>.Ok(data);
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

        [HttpPost, Route("suggest")]
        public async Task<BaseRes> Suggest(SuggessTeamReq req) {
            try {
                var res = await teamService.Suggestion(req);
                return BaseRes<ListTeamRes?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("auto-cal/{teamId}")]
        public async Task<BaseRes> Automation([FromRoute] string teamId, [FromQuery] EAutoMation type = EAutoMation.Location) {
            try {
                var res = await teamService.Automation(type, teamId);
                return BaseRes<object?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("suggest-user")]
        public async Task<BaseRes> SuggestUserForTeam(ListUserSuggest req) {
            try {
                var res = await teamService.SuggestUserForTeam(req);
                return BaseRes<ListUserRes?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}