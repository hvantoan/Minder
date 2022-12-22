using Microsoft.EntityFrameworkCore;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Services.Common;
using Minder.Services.Hashers;
using Minder.Services.Interfaces;
using Minder.Services.Models.User;
using Minder.Services.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Services.Implements {

    public class UserService : BaseService, IUserService {

        public UserService(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task<UserDto?> Get() {
            var user = await this.db.Users.AsNoTracking().FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            Role? role = null;
            if (!string.IsNullOrEmpty(user.RoleId)) {
                role = await this.db.Roles.AsNoTracking()
                    .Where(o => o.Id == user.RoleId && !o.IsDelete)
                    .FirstOrDefaultAsync();
            }
            return UserDto.FromEntity(user, role);
        }

        public async Task ChangePassword(string oldPassword, string newPassword) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);
            ManagedException.ThrowIf(!PasswordHashser.Verify(oldPassword, user.Password), Messages.User.ChangePassword.User_IncorrentOldPassword);

            user.Password = PasswordHashser.Hash(newPassword);
            await this.db.SaveChangesAsync();
        }

        public async Task ResetPassword(string password) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            user.Password = PasswordHashser.Hash(password);
            await this.db.SaveChangesAsync();
        }

        public async Task UpdateMe(UserDto model) {
            this.logger.Information($"{nameof(UserService)} - {nameof(Update)} - Start", model);

            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId && !o.IsDelete);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            user.Name = model.Name;
            await this.db.SaveChangesAsync();
        }

        public async Task<string> Create(UserDto model, ERoleType Role) {
            var existed = await this.db.Users.AnyAsync(o => o.Username == model.Username);

            ManagedException.ThrowIf(existed, Messages.User.CreateOrUpdate.User_Existed);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Username), Messages.User.CreateOrUpdate.User_UsernameRequired);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Password), Messages.User.CreateOrUpdate.User_PasswordRequired);

            User user = new() {
                Id = Guid.NewGuid().ToStringN(),
                Username = model.Username!,
                Name = model.Name,
                Avatar = "",
                IsAdmin = false,
                Password = PasswordHashser.Hash(model.Password!),
            };

            if (Role == ERoleType.Admin) user.RoleId = "469b14225a79448c93e4e780aa08f0cc";
            if (Role == ERoleType.User) user.RoleId = "6ffa9fa20755486d9e317d447b652bd8";
            await this.Validate(user.RoleId!);

            await this.db.Users.AddAsync(user);
            await this.db.SaveChangesAsync();
            return user.Id;
        }

        public async Task Update(UserDto model) {
            this.logger.Information($"{nameof(UserService)} - {nameof(Update)} - Start", model);

            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            user.Name = model.Name;
            await this.db.SaveChangesAsync();
        }

        private async Task Validate(string roleId) {
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