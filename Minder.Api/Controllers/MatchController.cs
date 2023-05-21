﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Interfaces;
using Minder.Service.Models.Match;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/matches")]
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

        [HttpPost]
        public async Task<BaseRes> List(ListMatchReq req) {
            try {
                var res = await matchService.List(req);
                return BaseRes<ListMatchRes>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("{matchId}")]
        public async Task<BaseRes> List([FromRoute] string matchId) {
            try {
                var res = await matchService.Get(matchId);
                return BaseRes<MatchDto?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("{matchId}/selected-time")]
        public async Task<BaseRes> UpdateTime([FromRoute] string matchId, UpdateTimeReq req) {
            try {
                var res = await matchService.Update(matchId, req.TeamId, req, Service.Enums.EUpdateType.Time);
                return BaseRes<MatchDto?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("{matchId}/selected-stadium")]
        public async Task<BaseRes> SelectedStadium([FromRoute] string matchId, UpdateStadiumReq req) {
            try {
                var res = await matchService.Update(matchId, req.TeamId, req, Service.Enums.EUpdateType.Stadium);
                return BaseRes<MatchDto?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}