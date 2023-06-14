using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.Auth;
using Minder.Service.Models.User;
using Minder.Services.Common;
using Minder.Services.Hashers;
using Minder.Services.Interfaces;
using Minder.Services.Models.Auth;
using Minder.Services.Models.User;
using Minder.Services.Resources;
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

        public AuthService(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.emailService = serviceProvider.GetRequiredService<IEmailService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
        }

        public async Task<LoginRes> WebLogin(LoginReq request) {
            var user = await this.db.Users.AsNoTracking().FirstOrDefaultAsync(o => o.Username == request.Username.ToLower().Trim() && !o.IsDeleted && o.IsActive);

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
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
            };
        }

        public async Task<LoginRes> Refresh() {
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
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
            };
        }

        public async Task Register(UserDto model) {
            await this.Validation(model);
            await this.userService.Create(model);
            var otp = await GenerateOTP(model.Username!);
            await this.emailService.SendOTP(otp, model.Username!);
        }

        public async Task ResendOTP(string username) {
            var otp = await GenerateOTP(username);
            await this.emailService.SendOTP(otp, username);
        }

        private async Task<string> GenerateOTP(string email, EVerifyType type = EVerifyType.Register) {
            var isExit = await this.db.Users.AnyAsync(o => o.Username == email);
            ManagedException.ThrowIf(isExit, Messages.User.User_NotFound);
            var userVerify = await this.db.RegistrationInformations.FirstOrDefaultAsync(o => o.Username == email);
            var otps = await this.db.RegistrationInformations.Select(o => o.OTPCode).ToListAsync();
            if (userVerify == null) {
                userVerify = new RegistrationInformation() {
                    Id = Guid.NewGuid().ToStringN(),
                    Username = email,
                    Type = type,
                    /// TODO: Handle Generate OTP code
                    OTPCode = "111111", // EMailHelper.GenarateOTP(),
                    CreateAt = DateTimeOffset.Now,
                };
                await this.db.RegistrationInformations.AddAsync(userVerify);
            } else {
                userVerify.CreateAt = DateTimeOffset.Now;
                userVerify.OTPCode = "111111";// EMailHelper.GenarateOTP();
            }
            await this.db.SaveChangesAsync();
            return userVerify.OTPCode;
        }

        public async Task ForgotPassword(ForgotPasswordReq request) {
            var isExited = await this.db.Users.AnyAsync(o => o.Username == request.Username);
            ManagedException.ThrowIf(!isExited, Messages.User.User_NotFound);

            var otp = await GenerateOTP(request.Username!, EVerifyType.ForgetPassword);
            await this.emailService.SendOTP(otp, request.Username!);
        }

        public async Task<VerifyRes> Verify(VerifyUserReq request) {
            var userVerify = await this.db.RegistrationInformations.FirstOrDefaultAsync(o => o.OTPCode == request.Code && o.Username == request.Username);
            ManagedException.ThrowIf(userVerify == null || userVerify.OTPCode != request.Code, Messages.Auth.Auth_IncorresctOTP);
            switch (userVerify!.Type) {
                case EVerifyType.Register:
                    var registerUser = await this.db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Username == userVerify.Username && !o.IsDeleted && !o.IsActive);
                    registerUser!.IsActive = true;
                    await this.db.SaveChangesAsync();
                    return new VerifyRes();

                case EVerifyType.ForgetPassword:
                    ManagedException.ThrowIf(string.IsNullOrWhiteSpace(userVerify.Username), Messages.System.System_Error);
                    var user = await this.db.Users.FirstOrDefaultAsync(o => o.Username == userVerify.Username);
                    var roleClaims = new List<Claim>();
                    string token = this.GenerateToken(user!.Id, user.Username, user.Name, roleClaims, DateTime.Now.AddMinutes(5));
                    return new VerifyRes() { Token = token };
            }
            return new VerifyRes() { Status = false };
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

            var user = await this.db.Users.AnyAsync(o => o.Username == model.Username && o.IsActive);
            ManagedException.ThrowIf(user, Messages.User.User_Existed);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Password), Messages.User.User_PasswordRequired);
            var userPasswordLenght = model.Password.Length;
            ManagedException.ThrowIf(userPasswordLenght < 8 || model.Password.Contains(' '), Messages.User.User_PasswordRequest);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.User.User_NameRequired);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Phone), Messages.User.User_PhoneRequired);
        }
    }
}