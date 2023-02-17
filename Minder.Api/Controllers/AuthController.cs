using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Models.Auth;
using Minder.Service.Models.User;
using Minder.Services.Interfaces;
using Minder.Services.Models;
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
        public async Task<BaseRes> Login(LoginReq request) {
            try {
                var response = await this.authService.WebLogin(request);
                return BaseRes<LoginRes>.Ok(response);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("register")]
        public async Task<BaseRes> Register(UserDto model) {
            try {
                await authService.Register(model);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("forgot-password")]
        public async Task<BaseRes> ForgotPassword(ForgotPasswordReq model) {
            try {
                await authService.ForgotPassword(model);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("login/refresh")]
        public async Task<BaseRes> Refresh() {
            try {
                var response = await this.authService.Refresh();
                return BaseRes<LoginRes>.Ok(response);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("verify")]
        public async Task<BaseRes> Verify(string otp) {
            try {
                var response = await authService.Verify(new VerifyUserReq() { Code = otp });
                return BaseRes<VerifyRes>.Ok(response);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet, Route("user/check")]
        public async Task<BaseRes> Check([FromQuery] string userName) {
            try {
                var response = await authService.CheckUser(userName);
                return BaseRes<UserNameValidate>.Ok(response);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}