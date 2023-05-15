using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Interfaces;
using Minder.Service.Models.Match;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/matchs")]
    public class MatchController : ControllerBase {
        private readonly IMatchService matchService;

        public MatchController(IMatchService matchService) {
            this.matchService = matchService;
        }

        [HttpPost, Route("swipe-card")]
        public async Task<BaseRes> SwipCard(CreateMatchReq req) {
            try {
                await matchService.SwipeCard(req);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}