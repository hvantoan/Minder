using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Interfaces;
using Minder.Service.Models.AppInfo;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Route("api/app-info")]
    public class AppInfoController : ControllerBase {
        private readonly IAppInfoService appInfoService;

        public AppInfoController(IAppInfoService appInfoService) {
            this.appInfoService = appInfoService;
        }

        [HttpGet, Route("ver")]
        public async Task<BaseRes> Get() {
            try {
                var response = await this.appInfoService.GetVer();
                return BaseRes<AppVer>.Ok(response);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("about-us")]
        public async Task<BaseRes> GetAbout() {
            try {
                var response = await this.appInfoService.GetAbout();
                return BaseRes<AppAbout>.Ok(response);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Authorize, Route("set")]
        public async Task<BaseRes> Set(string? ver, string? aboutUs, string? hotline) {
            try {
                await this.appInfoService.Set(ver, aboutUs, hotline);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}