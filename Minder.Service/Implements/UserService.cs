using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database.Enums;
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
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Services.Implements {

    public class UserService : BaseService, IUserService {
        public readonly ICacheManager cacheManager;
        public readonly IEmailService emailService;
        public readonly IFileService fileService;

        public UserService(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
            this.emailService = serviceProvider.GetRequiredService<IEmailService>();
            this.fileService = serviceProvider.GetRequiredService<IFileService>();
        }

        public async Task<UserDto?> Get(string? key) {
            var user = await this.db.Users.Include(o => o.GameSetting).AsNoTracking()
                .WhereIf(string.IsNullOrEmpty(key), o => o.Id == this.current.UserId)
                .WhereIf(!string.IsNullOrEmpty(key), o => o.Username.Contains(key) || o.Id == key)
                .FirstOrDefaultAsync();
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            var avatar = await this.fileService.Get(user.Id, EItemType.UserAvatar);
            var coverAvatar = await this.fileService.Get(user.Id, EItemType.UserCover);
            return UserDto.FromEntity(user, null, avatar, coverAvatar);
        }

        public async Task<string> Create(UserDto model) {
            this.logger.Information($"{nameof(User)} - {nameof(Create)} - Start", model);

            var isExited = await this.db.Users.AnyAsync(o => o.Username == model.Username && o.IsActive);
            ManagedException.ThrowIf(isExited, Messages.User.User_Existed);
            model.GameSetting ??= new();
            if (!model.GameSetting.GameTypes.Any()) model.GameSetting.GameTypes.Add(EGameType.Five);

            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Username == model.Username && !o.IsActive);

            if (user != null) {
                user.Name = model.Name;
                user.Phone = model.Phone;
                user.Password = PasswordHashser.Hash(model.Password!);
            } else {
                user = new() {
                    Id = Guid.NewGuid().ToStringN(),
                    Username = model.Username!.ToLower(),
                    Password = PasswordHashser.Hash(model.Password!),
                    Name = model.Name,
                    Phone = model.Phone,
                    Age = model.Age,
                    Sex = model.Sex,
                    GameSetting = new GameSetting() {
                        Id = Guid.NewGuid().ToStringN(),
                        GameTypes = JsonConvert.SerializeObject(model.GameSetting.GameTypes),
                        GameTime = JsonConvert.SerializeObject(model.GameSetting.GameTime),
                        Longitude = model.GameSetting.Longitude,
                        Latitude = model.GameSetting.Latitude,
                        Radius = model.GameSetting.Radius,
                        Rank = model.GameSetting.Rank,
                        Point = model.GameSetting.Point,
                    },
                    IsAdmin = false,
                    IsActive = false,
                    RoleId = "6ffa9fa20755486d9e317d447b652bd8"
                };
                await this.Validate(user.RoleId!);
                await this.db.Users.AddAsync(user);
            }

            await this.db.SaveChangesAsync();
            this.logger.Information($"{nameof(Team)} - {nameof(Create)} - End", model);
            return user.Id;
        }

        public async Task ChangePassword(ChangePasswordReq request) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);
            ManagedException.ThrowIf(!PasswordHashser.Verify(request.OldPassword, user.Password), Messages.User.User_IncorrentOldPassword);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(user.Password), Messages.User.User_PasswordRequired);
            var userPasswordLenght = user.Password.Length;
            ManagedException.ThrowIf(userPasswordLenght < 8 || user.Password.Contains(' '), Messages.User.User_PasswordRequest);

            user.Password = PasswordHashser.Hash(request.NewPassword);
            await this.db.SaveChangesAsync();
        }

        public async Task ResetPassword(string password) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(password), Messages.User.User_PasswordRequired);
            var userPasswordLenght = password.Length;
            ManagedException.ThrowIf(userPasswordLenght < 8 || password.Contains(' '), Messages.User.User_PasswordRequest);

            user.Password = PasswordHashser.Hash(password);
            await this.db.SaveChangesAsync();
        }

        public async Task UpdateMe(UserDto model) {
            this.logger.Information($"{nameof(UserService)} - {nameof(UpdateMe)} - Start", model);

            var user = await this.db.Users.Include(o => o.GameSetting).FirstOrDefaultAsync(o => o.Id == this.current.UserId && !o.IsDelete);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            if (!string.IsNullOrWhiteSpace(model.Name)) user.Name = model.Name;
            if (!string.IsNullOrWhiteSpace(model.Phone)) user.Phone = model.Phone;
            if (!string.IsNullOrWhiteSpace(model.Description)) user.Description = model.Description;
            if (model.Age > 0) user.Age = model.Age;
            if (model.Sex != ESex.Unknown && user.Sex != model.Sex) user.Sex = model.Sex;

            if (model.GameSetting != null) {
                user.GameSetting ??= new();
                user.GameSetting.Id ??= Guid.NewGuid().ToStringN();
                if (model.GameSetting.GameTime != null) user.GameSetting.GameTime = JsonConvert.SerializeObject(model.GameSetting.GameTime);
                if (model.GameSetting.GameTypes != null) user.GameSetting.GameTypes = JsonConvert.SerializeObject(model.GameSetting.GameTypes);
                if (model.GameSetting.Longitude != decimal.Zero) user.GameSetting.Longitude = model.GameSetting.Longitude;
                if (model.GameSetting.Latitude != decimal.Zero) user.GameSetting.Latitude = model.GameSetting.Latitude;
                if (model.GameSetting.Radius != 0.0) user.GameSetting.Radius = model.GameSetting.Radius;
                if (model.GameSetting.Rank != ERank.None) user.GameSetting.Rank = model.GameSetting.Rank;
            }
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