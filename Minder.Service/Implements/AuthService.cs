using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Minder.Database;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Services.Common;
using Minder.Services.Hashers;
using Minder.Services.Interfaces;
using SeoTool.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToolSeoViet.Database.Enums;
using ToolSeoViet.Services.Models.Auth;
using ToolSeoViet.Services.Resources;

namespace Minder.Services.Implements {

    public class AuthService : BaseService, IAuthService {
        private const string googleUrl = $"https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=";

        public AuthService(MinderContext db, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(db, httpContextAccessor, configuration) {
        }

        public async Task<LoginResponse> WebLogin(LoginRequest request) {

            var user = await this.db.Users.AsNoTracking().FirstOrDefaultAsync(o => o.Username == request.Username.ToLower().Trim());
            if (user == null)
                throw new ManagedException(Messages.Auth.Login.User_NotFound);
            if (!user.IsAdmin && !user.IsActive)
                throw new ManagedException(Messages.Auth.Login.User_Inactive);
            if (!PasswordHashser.Verify(request.Password, user.Password))
                throw new ManagedException(Messages.Auth.Login.User_IncorrectPassword);

            var permissions = await this.db.Permisstions.Where(o => o.Type == EPermission.Web).AsNoTracking().ToListAsync();
            Role role = null;

            if (!string.IsNullOrWhiteSpace(user.RoleId)) {
                role = await this.db.Roles.Include(o => o.RolePermissions).AsNoTracking().FirstOrDefaultAsync(o => o.Id == user.RoleId);
            }

            var userPermissions = UserPermissionDto.MapFromEntities(permissions, role?.RolePermissions?.ToList(), user.IsAdmin);
            var expiredAt = GetTokenExpiredAt();
            var claims = this.GetClaimPermissions(userPermissions);

            return new() {
                Token = this.GenerateToken(user.Id, user.Username, claims, expiredAt),
                ExpiredTime = new DateTimeOffset(expiredAt).ToUnixTimeMilliseconds(),
                Username = user.Username,
            };
        }


        public async Task<LoginResponse> WebLoginGoogle(LoginGoogleRequest request) {
            var permissions = await this.db.Permisstions.Where(o => o.Type == EPermission.Web).AsNoTracking().ToListAsync();
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
                        if (request.ExternalId != googleObj.Id) {
                            throw new ManagedException(Messages.Auth.Login.User_NotFound);
                        }
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
                    IsActive = true,
                    RoleId = "469b14225a79448c93e4e780aa08f0cc"
                };
                db.Users.Add(userExists);
                db.SaveChanges();

            }

            User user = this.db.Users.FirstOrDefault(o => o.Username == request.Email) ?? new User() { Id = "" };

            if (user == null)
                throw new ManagedException(Messages.Auth.Login.User_NotFound);
            if (!user.IsAdmin && !user.IsActive)
                throw new ManagedException(Messages.Auth.Login.User_Inactive);

            Role role = null;
            if (!string.IsNullOrWhiteSpace(user.RoleId) && !string.IsNullOrEmpty(user.RoleId)) {
                role = await this.db.Roles.Include(o => o.RolePermissions).AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == user.RoleId) ?? new Role() { Id = "" };
            }

            var userPermissions = UserPermissionDto.MapFromEntities(permissions, role?.RolePermissions?.ToList(), user.IsAdmin);
            var expiredAt = GetTokenExpiredAt();
            var claims = this.GetClaimPermissions(userPermissions);

            return new() {
                Token = this.GenerateToken(user.Id, user.Username, claims, expiredAt),
                ExpiredTime = new DateTimeOffset(expiredAt).ToUnixTimeMilliseconds(),
                Username = user.Username,
            };
        }


        private static DateTime GetTokenExpiredAt() {
            var now = DateTime.Now;
            var midnight = now.AddDays(1).Date;
            return midnight.Subtract(now).TotalMinutes > 60 ? midnight : midnight.AddDays(1);
        }

        private string GenerateToken(string userId, string username, List<Claim> roleClaims, DateTime expiredAt) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JwtSecret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>() {
                new Claim("UserId", userId),
                new Claim("Username", username)
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
                claims.Add(new Claim(ClaimTypes.Role, item.ClaimName));

                if (item.Items != null && item.Items.Any()) {
                    claims.AddRange(this.GetClaimPermissions(item.Items));
                }
            }
            return claims;
        }
    }
}