using Microsoft.EntityFrameworkCore;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.User;
using Minder.Services.Common;
using Minder.Services.Hashers;
using Minder.Services.Interfaces;
using Minder.Services.Models.User;
using Minder.Services.Resources;
using System;
using System.ComponentModel.DataAnnotations;
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

        public async Task<string> Create(UserDto model) {
            User user = new() {
                Id = Guid.NewGuid().ToStringN(),
                Username = model.Username!.ToLower(),
                Name = model.Name,
                Avatar = "",
                IsAdmin = false,
                IsActive = false,
                Password = PasswordHashser.Hash(model.Password!),
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
            this.logger.Information($"{nameof(UserService)} - {nameof(Update)} - Start", model);

            await ValidateInfo(model);
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId && !o.IsDelete);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            user.Name = model.Name;
            await this.db.SaveChangesAsync();
        }

        private async Task ValidateInfo(UserDto model) {
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Username), Messages.User.User_UsernameRequired);
            var userNameLenght = model.Username.Length;
            ManagedException.ThrowIf(2 <= userNameLenght && userNameLenght <= 20 && new EmailAddressAttribute().IsValid(model.Username), Messages.User.User_UsernameRequest);
            var user = await this.db.Users.AnyAsync(o => !o.IsDelete && o.Username == model.Username);
            ManagedException.ThrowIf(user, Messages.User.User_Existed);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Password), Messages.User.User_PasswordRequired);
            var userPasswordLenght = model.Password.Length;
            ManagedException.ThrowIf(userPasswordLenght > 8 && model.Password.Contains(' '), Messages.User.User_PasswordRequest);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.User.User_NameRequired);
            var isUnique = await this.db.Users.AnyAsync(o => !o.IsDelete && o.Username == model.Username);
            ManagedException.ThrowIf(!isUnique, Messages.User.User_Existed);
        }

        public async Task Update(UserDto model) {
            this.logger.Information($"{nameof(UserService)} - {nameof(Update)} - Start", model);

            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            user.Name = model.Name;
            await this.db.SaveChangesAsync();
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