using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Minder.Database;
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
using System.Threading.Tasks;

namespace Minder.Services.Implements {

    public class UserService : BaseService, IUserService {

        public UserService(MinderContext db, IHttpContextAccessor httpContextAccessor)
            : base(db, httpContextAccessor) {
        }

        public async Task ChangePassword(string oldPassword, string newPassword) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.currentUserId);
            if (user == null)
                throw new ManagedException(Messages.User.User_NotFound);

            if (!PasswordHashser.Verify(oldPassword, user.Password))
                throw new ManagedException(Messages.User.ChangePassword.User_IncorrentOldPassword);

            user.Password = PasswordHashser.Hash(newPassword);

            await this.db.SaveChangesAsync();
        }

        public async Task ResetPassword(string password) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.currentUserId);
            if (user == null)
                throw new ManagedException(Messages.User.User_NotFound);

            user.Password = PasswordHashser.Hash(password);

            await this.db.SaveChangesAsync();
        }

        public async Task Create(UserDto model, ERoleType Role) {
            var existed = await this.db.Users.AnyAsync(o => o.Username == model.Username);
            if (existed)
                throw new ManagedException(Messages.User.CreateOrUpdate.User_Existed);

            if (Role == ERoleType.Admin) model.RoleId = "469b14225a79448c93e4e780aa08f0cc";
            if (Role == ERoleType.User) model.RoleId = "6ffa9fa20755486d9e317d447b652bd8";

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
        }

        public async Task Update(UserDto model) {
            var existed = await this.db.Users.AnyAsync(o => o.Id != this.currentUserId && o.Username == model.Username);
            if (existed)
                throw new ManagedException(Messages.User.CreateOrUpdate.User_Existed);

            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.currentUserId);
            if (user == null)
                throw new ManagedException(Messages.User.User_NotFound);

            if (user.IsAdmin && !model.IsActive)
                throw new ManagedException(Messages.User.CreateOrUpdate.User_NotInactive);

            user.Username = model.Username;
            await this.db.SaveChangesAsync();
        }

        private async Task Validate(string roleId) {
            if (string.IsNullOrWhiteSpace(roleId))
                throw new ManagedException(Messages.User.User_NotFound);

            var role = await this.db.Roles.FirstOrDefaultAsync(o => o.Id == roleId);
            if (role == null)
                throw new ManagedException(Messages.User.User_NotFound);
        }

        public async Task<UserDto> Get() {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.currentUserId);
            if (user == null) throw new ManagedException(Messages.User.User_NotFound);
            return UserDto.FromEntity(user);
        }
    }
}