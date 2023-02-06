using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Models.User;
using Minder.Services.Interfaces;
using Minder.Services.Models;
using Minder.Services.Models.User;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/user")]
    public class UserController : ControllerBase {
        private readonly IUserService userService;

        public UserController(IUserService userService) {
            this.userService = userService;
        }

        [HttpGet, Route("get")]
        public async Task<BaseRes> Get([FromQuery] string? key) {
            try {
                var data = await userService.Get(key);
                return BaseRes<UserDto>.Ok(data!);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("save")]
        public async Task<BaseRes> Save(UserDto model) {
            try {
                await userService.UpdateMe(model);
                return BaseRes.Ok();
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