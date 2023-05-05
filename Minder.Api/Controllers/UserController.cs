using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Models.User;
using Minder.Services.Interfaces;
using Minder.Services.Models;
using Minder.Services.Models.User;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/users")]
    public class UserController : ControllerBase {
        private readonly IUserService userService;

        public UserController(IUserService userService) {
            this.userService = userService;
        }

        [HttpGet, Route("{userId}")]
        public async Task<BaseRes> Get(string? userId) {
            try {
                var data = await userService.Get(userId);
                return BaseRes<UserDto?>.Ok(data);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("me")]
        public async Task<BaseRes> GetMe() {
            try {
                var data = await userService.Get(null);
                return BaseRes<UserDto?>.Ok(data);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("save")]
        public async Task<BaseRes> Save(UserDto model) {
            try {
                var res = await userService.UpdateMe(model);
                return BaseRes<UserDto?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("change-password")]
        public async Task<BaseRes> ChangePassword(ChangePasswordReq request) {
            try {
                await userService.ChangePassword(request);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("reset-password")]
        public async Task<BaseRes> ResetPassword([FromQuery] string password) {
            try {
                await userService.ResetPassword(password);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}