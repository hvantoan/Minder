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
        public async Task<BaseResponse> Get([FromQuery] string? key) {
            try {
                var data = await userService.Get(key);
                return BaseResponse<UserDto>.Ok(data!);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("save")]
        public async Task<BaseResponse> Save(UserDto model) {
            try {
                await userService.UpdateMe(model);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("change-password")]
        public async Task<BaseResponse> ChangePassword(ChangePasswordRequest request) {
            try {
                await userService.ChangePassword(request);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
    }
}