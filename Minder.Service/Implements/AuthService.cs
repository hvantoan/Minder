using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Service.Interfaces;
using Minder.Service.Models.Auth;
using Minder.Service.Models.User;
using Minder.Services.Common;
using Minder.Services.Hashers;
using Minder.Services.Interfaces;
using Minder.Services.Models.Auth;
using Minder.Services.Models.User;
using Minder.Services.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToolSeoViet.Database.Enums;

namespace Minder.Services.Implements {

    public class AuthService : BaseService, IAuthService {
        private readonly IEmailService emailService;
        private readonly IUserService userService;
        private readonly ICacheManager cacheManager;

        public AuthService(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.emailService = serviceProvider.GetRequiredService<IEmailService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
        }

        public async Task<LoginResponse> WebLogin(LoginRequest request) {
            var user = await this.db.Users.AsNoTracking().FirstOrDefaultAsync(o => o.Username == request.Username.ToLower().Trim() && !o.IsDelete);

            ManagedException.ThrowIf(user == null, Messages.Auth.Auth_NotFound);
            ManagedException.ThrowIf(!PasswordHashser.Verify(request.Password, user.Password), Messages.Auth.Auth_IncorrectPassword);

            var permissions = await this.db.Permissions.Where(o => o.Type == EPermission.Web).AsNoTracking().ToListAsync();
            Role? role = null;

            if (!string.IsNullOrWhiteSpace(user.RoleId)) {
                role = await this.db.Roles.Include(o => o.RolePermissions).AsNoTracking().FirstOrDefaultAsync(o => o.Id == user.RoleId);
            }

            var userPermissions = UserPermissionDto.MapFromEntities(permissions, role?.RolePermissions?.ToList(), user.IsAdmin);
            ManagedException.ThrowIf(!userPermissions.Any(o => o.IsEnable), Messages.Auth.Auth_NoPermission);
            var claims = this.GetClaimPermissions(userPermissions);

            return new() {
                Token = this.GenerateToken(user.Id, user.Username, user.Name, claims, GetTokenExpiredAt(1)),
                RefreshToken = this.GenerateToken(user.Id, user.Username, user.Name, claims, GetTokenExpiredAt(30)),
                Name = user.Name,
                Username = user.Username,
            };
        }

        public async Task<LoginResponse> Refresh() {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.Auth.Auth_NotFound);

            var permissions = await this.db.Permissions.Where(o => o.Type == EPermission.Web).AsNoTracking().ToListAsync();
            Role? role = null;
            if (!string.IsNullOrWhiteSpace(user.RoleId)) {
                role = await this.db.Roles.Include(o => o.RolePermissions).AsNoTracking().FirstOrDefaultAsync(o => o.Id == user.RoleId);
            }

            var userPermissions = UserPermissionDto.MapFromEntities(permissions, role?.RolePermissions?.ToList(), user.IsAdmin);
            var claims = this.GetClaimPermissions(userPermissions);

            return new() {
                Token = this.GenerateToken(user.Id, user.Username, user.Name, claims, GetTokenExpiredAt(1)),
                Username = user.Username,
                Name = user.Name,
            };
        }

        public async Task Register(UserDto model) {
            await this.Validation(model);
            await this.emailService.SendOTP(model, model.Username!);
        }

        public async Task ForgotPassword(ForgotPasswordReq request) {
            var isExited = await this.db.Users.AnyAsync(o => o.Username == request.Username);
            ManagedException.ThrowIf(!isExited, Messages.User.User_NotFound);

            await this.emailService.SendOTP(request, request.Username!);
        }

        public async Task<VerifyResponse> Verify(Verify verify) {
            var response = cacheManager.VerifyOTP(verify);

            switch (verify.Type) {
                case EVerifyType.Register:
                    await userService.Create(JsonConvert.DeserializeObject<UserDto>(response)!);
                    return new VerifyResponse();

                case EVerifyType.ForgetPassword:
                    var user = await this.db.Users.FirstOrDefaultAsync(o => o.Username == verify.Username);
                    var roleClaims = new List<Claim>();

                    string token = this.GenerateToken(user!.Id, user.Username, user.Name, roleClaims, DateTime.Now.AddMinutes(5));
                    return new VerifyResponse() { Token = token };
            }
            return new VerifyResponse() { Status = false };
        }

        public async Task<UserNameValidate> CheckUser(string userName) {
            var isExit = await this.db.Users.AnyAsync(o => o.Username == userName);

            if (isExit) return new UserNameValidate() {
                Status = false,
                Message = ValidateMessage.IsNotValid
            };

            return new UserNameValidate() {
                Status = true,
                Message = ValidateMessage.IsValid,
            };
        }

        private static DateTime GetTokenExpiredAt(int day) {
            var now = DateTime.Now;
            return now.AddDays(day).Date;
        }

        private string GenerateToken(string userId, string username, string name, List<Claim> roleClaims, DateTime expiredAt) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JwtSecret"] ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>() {
                new Claim(Constant.TokenUserId, userId),
                new Claim(Constant.TokenUserName, username),
                new Claim(Constant.TokenName, name)
            };

            claims.AddRange(roleClaims);

            var token = new JwtSecurityToken(
              claims: claims,
              expires: expiredAt,
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private List<Claim> GetClaimPermissions(List<UserPermissionDto> permissions) {
            List<Claim> claims = new();
            foreach (var item in permissions) {
                if (!item.IsEnable) continue;

                if (item.IsClaim)
                    claims.Add(new Claim(ClaimTypes.Role, item.ClaimName));

                if (item.Items != null && item.Items.Any()) {
                    claims.AddRange(this.GetClaimPermissions(item.Items));
                }
            }
            return claims;
        }

        private async Task Validation(UserDto model) {
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Username), Messages.User.User_UsernameRequired);
            ManagedException.ThrowIf(!(new EmailAddressAttribute().IsValid(model.Username)), Messages.User.User_UsernameRequest);

            var user = await this.db.Users.AnyAsync(o => !o.IsDelete && o.Username == model.Username);
            ManagedException.ThrowIf(user, Messages.User.User_Existed);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Password), Messages.User.User_PasswordRequired);
            var userPasswordLenght = model.Password.Length;
            ManagedException.ThrowIf(userPasswordLenght < 8 || model.Password.Contains(' '), Messages.User.User_PasswordRequest);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.User.User_NameRequired);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Phone), Messages.User.User_PhoneRequired);
        }
    }
}