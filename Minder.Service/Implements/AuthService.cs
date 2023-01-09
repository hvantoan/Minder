using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Interfaces;
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
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToolSeoViet.Database.Enums;

namespace Minder.Services.Implements {

    public class AuthService : BaseService, IAuthService {
        private readonly IEmailService emailService;
        private readonly IUserService userService;
        private readonly ICacheManager cacheManager;
        private const string googleUrl = $"https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=";

        public AuthService(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.emailService = serviceProvider.GetRequiredService<IEmailService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
        }

        public async Task<string> Register(UserDto model) {
            this.logger.Information($"{nameof(UserService)} - {nameof(Register)} - Start", model);
            await this.Validation(model);
            ManagedException.ThrowIf(!this.cacheManager.VerifyOtp(model.Username!, this.current.OTP), Messages.User.User_OTPIncorect);

            return await this.userService.Create(model);
        }

        public async Task<bool> VerifyUser(UserDto model) {
            await this.Validation(model);
            return await this.emailService.SendOTP(model);
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
        }

        public async Task<LoginResponse> WebLogin(LoginRequest request) {
            var user = await this.db.Users.AsNoTracking().FirstOrDefaultAsync(o => o.Username == request.Username.ToLower().Trim() && !o.IsDelete);

            ManagedException.ThrowIf(user == null, Messages.Auth.User_NotFound);
            ManagedException.ThrowIf(!PasswordHashser.Verify(request.Password, user.Password), Messages.Auth.User_IncorrectPassword);

            var permissions = await this.db.Permissions.Where(o => o.Type == EPermission.Web).AsNoTracking().ToListAsync();
            Role? role = null;

            if (!string.IsNullOrWhiteSpace(user.RoleId)) {
                role = await this.db.Roles.Include(o => o.RolePermissions).AsNoTracking().FirstOrDefaultAsync(o => o.Id == user.RoleId);
            }

            var userPermissions = UserPermissionDto.MapFromEntities(permissions, role?.RolePermissions?.ToList(), user.IsAdmin);
            ManagedException.ThrowIf(!userPermissions.Any(o => o.IsEnable), Messages.Auth.User_NoPermission);
            var claims = this.GetClaimPermissions(userPermissions);

            return new() {
                Token = this.GenerateToken(user.Id, user.Username, user.Name, claims, GetTokenExpiredAt(1)),
                RefreshToken = this.GenerateToken(user.Id, user.Username, user.Name, claims, GetTokenExpiredAt(30)),
                ExpiredTime = new DateTimeOffset(GetTokenExpiredAt(1)).ToUnixTimeMilliseconds(),
                Username = user.Username,
            };
        }

        public async Task<LoginResponse> WebLoginGoogle(LoginGoogleRequest request) {
            var permissions = await this.db.Permissions.Where(o => o.Type == EPermission.Web).AsNoTracking().ToListAsync();
            string url = googleUrl + request.ExternalToken;
            using (HttpClient client = new()) {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                try {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode) {
                        var data = await response.Content.ReadAsStringAsync();
                        GoolgeUserInforModel googleObj = Newtonsoft.Json.JsonConvert.DeserializeObject<GoolgeUserInforModel>(data) ?? new GoolgeUserInforModel() { Id = "" };
                        ManagedException.ThrowIf(request.ExternalId != googleObj.Id, Messages.Auth.User_NotFound);
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
            User userExists = db.Users.FirstOrDefault(o => o.Username == request.Email) ?? new User() { Id = "" };
            if (userExists.Id.IsNullOrEmpty()) {
                userExists = new User() {
                    Id = Guid.NewGuid().ToStringN(),
                    Username = request.Email,
                    IsAdmin = false,
                    Avatar = "",
                    Name = "",
                    Password = "",
                    RoleId = "469b14225a79448c93e4e780aa08f0cc"
                };
                db.Users.Add(userExists);
                db.SaveChanges();
            }

            User user = this.db.Users.FirstOrDefault(o => o.Username == request.Email) ?? new User() { Id = "" };
            ManagedException.ThrowIf(user == null, Messages.Auth.User_NotFound);

            Role? role = null;
            if (!string.IsNullOrWhiteSpace(user.RoleId) && !string.IsNullOrEmpty(user.RoleId)) {
                role = await this.db.Roles.Include(o => o.RolePermissions).AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == user.RoleId) ?? new Role() { Id = "" };
            }

            var userPermissions = UserPermissionDto.MapFromEntities(permissions, role?.RolePermissions?.ToList(), user.IsAdmin);
            var expiredAt = GetTokenExpiredAt(1);
            var claims = this.GetClaimPermissions(userPermissions);

            return new() {
                Token = this.GenerateToken(user.Id, user.Username, user.Name, claims, expiredAt),
                RefreshToken = this.GenerateToken(user.Id, user.Username, user.Name, claims, GetTokenExpiredAt(30)),
                ExpiredTime = new DateTimeOffset(expiredAt).ToUnixTimeMilliseconds(),
                Username = user.Username,
                Name = user.Name,
            };
        }

        public async Task<LoginResponse> Refresh() {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.Auth.User_NotFound);

            var permissions = await this.db.Permissions.Where(o => o.Type == EPermission.Web).AsNoTracking().ToListAsync();
            Role? role = null;
            if (!string.IsNullOrWhiteSpace(user.RoleId)) {
                role = await this.db.Roles.Include(o => o.RolePermissions).AsNoTracking().FirstOrDefaultAsync(o => o.Id == user.RoleId);
            }

            var userPermissions = UserPermissionDto.MapFromEntities(permissions, role?.RolePermissions?.ToList(), user.IsAdmin);
            var claims = this.GetClaimPermissions(userPermissions);

            return new() {
                Token = this.GenerateToken(user.Id, user.Username, user.Name, claims, GetTokenExpiredAt(1)),
                ExpiredTime = new DateTimeOffset(GetTokenExpiredAt(1)).ToUnixTimeMilliseconds(),
                Username = user.Username,
                Name = user.Name,
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
    }
}