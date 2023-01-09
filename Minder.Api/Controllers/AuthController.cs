using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Models.Auth;
using Minder.Service.Models.User;
using Minder.Services.Interfaces;
using Minder.Services.Models;
using Minder.Services.Models.Auth;
using Minder.Services.Models.User;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, AllowAnonymous, Route("api/auth")]
    public class AuthController : ControllerBase {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService) {
            this.authService = authService;
        }

        [HttpPost, Route("login")]
        public async Task<BaseResponse> Login(LoginRequest request) {
            try {
                var response = await this.authService.WebLogin(request);
                return BaseResponse<LoginResponse>.Ok(response);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("register")]
        public async Task<BaseResponse> Register(UserDto model) {
            try {
                await authService.Register(model);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("forgot-password")]
        public async Task<BaseResponse> ForgotPassword(ForgotPasswordRequest model) {
            try {
                await authService.ForgotPassword(model);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpGet, Route("login/refresh")]
        public async Task<BaseResponse> Refresh() {
            try {
                var response = await this.authService.Refresh();
                return BaseResponse<LoginResponse>.Ok(response);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("verify")]
        public async Task<BaseResponse> Verify(Verify verify) {
            try {
                await authService.Verify(verify);
                return BaseResponse.Ok();
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpGet, Route("user/check")]
        public async Task<BaseResponse> Check([FromQuery] string userName) {
            try {
                var response = await authService.CheckUser(userName);
                return BaseResponse<UserNameValidate>.Ok(response);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
    }
}