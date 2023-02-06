using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Interfaces;
using Minder.Service.Models.AppInfo;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, AllowAnonymous, Route("api/app-info")]
    public class AppInfoController : ControllerBase {
        private readonly IAppInfoService appInfoService;

        public AppInfoController(IAppInfoService appInfoService) {
            this.appInfoService = appInfoService;
        }

        [HttpGet, Route("ver")]
        public async Task<BaseRes> Get() {
            try {
                var response = await this.appInfoService.Get();
                return BaseRes<AppInfo>.Ok(response);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("set")]
        public async Task<BaseRes> Set(string ver) {
            try {
                await this.appInfoService.Set(ver);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}