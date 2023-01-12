using Microsoft.EntityFrameworkCore;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.User;
using Minder.Services.Common;
using Minder.Services.Extensions;
using Minder.Services.Hashers;
using Minder.Services.Interfaces;
using Minder.Services.Models.User;
using Minder.Services.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Services.Implements {

    public class UserService : BaseService, IUserService {
        public readonly ICacheManager cacheManager;
        public readonly IEmailService emailService;

        public UserService(IServiceProvider serviceProvider, ICacheManager cacheManager, IEmailService emailService) : base(serviceProvider) {
            this.cacheManager = cacheManager;
            this.emailService = emailService;
        }

        public async Task<UserDto?> Get(string? key) {
            var user = await this.db.Users.AsNoTracking()
                .WhereIf(string.IsNullOrEmpty(key), o => o.Id == this.current.UserId)
                .WhereIf(!string.IsNullOrEmpty(key), o => o.Username == key)
                .FirstAsync();
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            return UserDto.FromEntity(user);
        }

        public async Task<string> Create(UserDto model) {
            User user = new() {
                Id = Guid.NewGuid().ToStringN(),
                Username = model.Username!.ToLower(),
                Password = PasswordHashser.Hash(model.Password!),
                Name = model.Name,
                Phone = model.Phone,
                Age = model.Age,
                Sex = model.Sex,
                GameType = model.GameType,
                GameTime = model.GameTime,
                Longitude = model.Longitude,
                Latitude = model.Latitude,
                Radius = model.Radius,
                Rank = model.Rank,
                Point = model.Point,
                IsAdmin = false,
                RoleId = "6ffa9fa20755486d9e317d447b652bd8"
            };
            await this.Validate(user.RoleId);

            await this.db.Users.AddAsync(user);
            await this.db.SaveChangesAsync();
            return user.Id;
        }

        public async Task ChangePassword(ChangePasswordRequest request) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);
            ManagedException.ThrowIf(!PasswordHashser.Verify(request.OldPassword, user.Password), Messages.User.User_IncorrentOldPassword);

            user.Password = PasswordHashser.Hash(request.NewPassword);
            await this.db.SaveChangesAsync();
        }

        public async Task ResetPassword(ForgotPasswordRequest request) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Username == request.Username);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            user.Password = PasswordHashser.Hash(request.Password);
            await this.db.SaveChangesAsync();
        }

        public async Task UpdateMe(UserDto model) {
            this.logger.Information($"{nameof(UserService)} - {nameof(UpdateMe)} - Start", model);

            await ValidateInfo(model);
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId && !o.IsDelete);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            user.Name = model.Name;
            user.Phone = model.Phone;
            user.Age = model.Age;
            user.Phone = model.Phone;
            user.Sex = model.Sex;
            user.Description = model.Description;

            user.GameType = model.GameType;
            user.GameTime = model.GameTime;
            user.Longitude = model.Longitude;
            user.Latitude = model.Latitude;
            user.Radius = model.Radius;


            await this.db.SaveChangesAsync();
        }

        private async Task ValidateInfo(UserDto model) {
            var user = await this.db.Users.AnyAsync(o => !o.IsDelete && o.Username == model.Username);
            ManagedException.ThrowIf(user, Messages.User.User_Existed);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.User.User_NameRequired);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Phone), Messages.User.User_PhoneRequired);
        }

        public async Task Validate(string roleId) {
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(roleId), Messages.User.User_NotFound);

            var role = await this.db.Roles.FirstOrDefaultAsync(o => o.Id == roleId);
            ManagedException.ThrowIf(role == null, Messages.User.User_NotFound);
        }

        public async Task Delete() {
            var user = await this.db.Users.Where(o => o.Id == this.current.UserId && !o.IsDelete).FirstOrDefaultAsync();
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);
            user.IsDelete = true;
            await this.db.SaveChangesAsync();
        }
    }
}