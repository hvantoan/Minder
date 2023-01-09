using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost, Route("login/google")]
        public async Task<BaseResponse> LoginGoogle(LoginGoogleRequest request) {
            try {
                var response = await this.authService.WebLoginGoogle(request);
                return BaseResponse<LoginResponse>.Ok(response);
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

        [HttpPost, Route("register")]
        public async Task<BaseResponse> Register(UserDto model) {
            try {
                var response = await authService.Register(model);
                return BaseResponse<string>.Ok(response);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }

        [HttpPost, Route("register/check")]
        public async Task<BaseResponse> Check(UserDto model) {
            try {
                var response = await authService.VerifyUser(model);
                return BaseResponse<bool>.Ok(response);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
    }
}