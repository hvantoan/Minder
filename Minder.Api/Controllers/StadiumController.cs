using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Database.Models;
using Minder.Service.Interfaces;
using Minder.Service.Models.Stadium;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/stadium")]
    public class StadiumController : ControllerBase {
        private readonly IStadiumService stadiumService;

        public StadiumController(IStadiumService stadiumService) {
            this.stadiumService = stadiumService;
        }

        [HttpGet, Route("get/{id}")]
        public async Task<BaseRes> Get(string id) {
            try {
                var res = await stadiumService.Get(new GetStadiumReq() { Id = id });
                return BaseRes<StadiumDto?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("list")]
        public async Task<BaseRes> List(ListStadiumReq req) {
            try {

                var res = await stadiumService.List(req);
                return BaseRes<ListStadiumRes>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("save")]
        public async Task<BaseRes> Save(StadiumDto model) {
            try {
                var res = await stadiumService.CreateOrUpdate(model);
                return BaseSaveRes<Stadium>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("init")]
        public async Task<BaseRes> InitStadiumData() {
            try {
                await stadiumService.InitialData();
                return BaseRes.Ok();    
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}