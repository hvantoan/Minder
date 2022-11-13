using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Minder.Database;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Services.Common;
using Minder.Services.Hashers;
using Minder.Services.Interfaces;
using Minder.Services.Models.User;
using Minder.Services.Resources;
using System;
using System.Threading.Tasks;

namespace Minder.Services.Implements {

    public class UserService : BaseService, IUserService {

        public UserService(MinderContext db, IHttpContextAccessor httpContextAccessor)
            : base(db, httpContextAccessor) {
        }

        public async Task ChangePassword(string oldPassword, string newPassword) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.currentUserId);
            if (user == null)
                throw new ManagedException(Messages.User.ChangePassword.User_NotFound);

            if (!PasswordHashser.Verify(oldPassword, user.Password))
                throw new ManagedException(Messages.User.ChangePassword.User_IncorrentOldPassword);

            user.Password = PasswordHashser.Hash(newPassword);

            await this.db.SaveChangesAsync();
        }

        public async Task ResetPassword(string userId, string password) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == userId);
            if (user == null)
                throw new ManagedException(Messages.User.ResetPassword.User_NotFound);

            user.Password = PasswordHashser.Hash(password);

            await this.db.SaveChangesAsync();
        }

        public async Task<UserDto> CreateOrUpdate(UserDto model) {
            model.Username = model.Username.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(model.Id)) {
                return await this.Create(model);
            }
            return await this.Update(model);
        }

        private async Task<UserDto> Create(UserDto model) {
            var existed = await this.db.Users.AnyAsync(o => o.Username == model.Username);
            if (existed)
                throw new ManagedException(Messages.User.CreateOrUpdate.User_Existed);

            await this.Validate(model.RoleId);

            User user = new() {
                Id = Guid.NewGuid().ToStringN(),
                RoleId = model.RoleId,
                Username = model.Username,
                IsActive = model.IsActive,
                Name = model.Name,
                Avatar = "",
                IsAdmin = false,
                Password = PasswordHashser.Hash(model.Password),
            };
            await this.db.Users.AddAsync(user);
            await this.db.SaveChangesAsync();

            return UserDto.FromEntity(user);
        }

        private async Task<UserDto> Update(UserDto model) {
            var existed = await this.db.Users.AnyAsync(o => o.Id != model.Id && o.Username == model.Username);
            if (existed)
                throw new ManagedException(Messages.User.CreateOrUpdate.User_Existed);

            await this.Validate(model.RoleId);

            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == model.Id);
            if (user == null)
                throw new ManagedException(Messages.User.CreateOrUpdate.User_NotFound);

            if (user.IsAdmin && !model.IsActive)
                throw new ManagedException(Messages.User.CreateOrUpdate.User_NotInactive);

            user.Username = model.Username;
            user.RoleId = model.RoleId;
            user.IsActive = model.IsActive;

            await this.db.SaveChangesAsync();

            return UserDto.FromEntity(user);
        }

        private async Task Validate(string roleId) {
            if (string.IsNullOrWhiteSpace(roleId))
                throw new ManagedException(Messages.User.CreateOrUpdate.Role_NotFound);

            var role = await this.db.Roles.FirstOrDefaultAsync(o => o.Id == roleId);
            if (role == null)
                throw new ManagedException(Messages.User.CreateOrUpdate.Role_NotFound);
        }
    }
}