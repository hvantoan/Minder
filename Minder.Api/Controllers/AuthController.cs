using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Services.Interfaces;
using Minder.Services.Models;
using Minder.Services.Models.Auth;
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
                var response = await this.authService.LoginWithRefreshToken();
                return BaseResponse<LoginResponse>.Ok(response);
            } catch (Exception ex) {
                return BaseResponse.Fail(ex.Message);
            }
        }
    }
}
