using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Models.User;
using Minder.Services.Interfaces;
using Minder.Services.Models;
using Minder.Services.Models.User;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {
    [ApiController, Authorize ,Route("api/user")]
    public class UserController : ControllerBase {
        private readonly IUserService userService;

        public UserController(IUserService userService) {
            this.userService = userService;
        }

        [HttpGet, Route("get")]
        public async Task<BaseResponse> Get() {
            try {
                var data = await userService.Get();
                return BaseResponse<UserDto>.Ok(data);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("save")]
        public async Task<BaseResponse> Save(UserDto model) {
            try {   
              await userService.Update(model);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
        [HttpPost, Route("create")]
        public async Task<BaseResponse> Create(CreateUserRequest request) {
            try {
                await userService.Create(request.User, request.Role);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("reset-password")]
        public async Task<BaseResponse> ResetPassword(ResetPasswordRequest request) {
            try {
                await userService.ResetPassword(request.Password);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
    }
}