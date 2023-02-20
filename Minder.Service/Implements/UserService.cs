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
            var user = await this.db.Users.AsNoTracking()
                .WhereIf(string.IsNullOrEmpty(key), o => o.Id == this.current.UserId)
                .WhereIf(!string.IsNullOrEmpty(key), o => o.Username == key)
                .FirstAsync();
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            var avatar = await this.fileService.Get(user.Id, EItemType.UserAvatar);
            var coverAvatar = await this.fileService.Get(user.Id, EItemType.UserCover);
            return UserDto.FromEntity(user, null, avatar, coverAvatar);
        }

        public async Task<string> Create(UserDto model) {
            this.logger.Information($"{nameof(User)} - {nameof(Create)} - Start", model);

            var isExited = await this.db.Users.AnyAsync(o => o.Username == model.Username);
            ManagedException.ThrowIf(isExited, Messages.User.User_Existed);

            User user = new() {
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
                    GameTimes = JsonConvert.SerializeObject(model.GameSetting.GameTimes),
                    Longitude = model.GameSetting.Longitude,
                    Latitude = model.GameSetting.Latitude,
                    Radius = model.GameSetting.Radius,
                    Rank = model.GameSetting.Rank,
                    Point = model.GameSetting.Point,
                },
                IsAdmin = false,
                RoleId = "6ffa9fa20755486d9e317d447b652bd8"
            };
            await this.Validate(user.RoleId);

            if (model.CoverAvatar?.Data != null && model.CoverAvatar.Data.Any()) {
                model.CoverAvatar.ItemId = user.Id;
                model.CoverAvatar.Type = EFile.Image;
                model.CoverAvatar.ItemType = EItemType.UserCover;
                await this.fileService.CreateOrUpdate(model.CoverAvatar!);
            }

            if (model.Avatar?.Data != null && model.Avatar.Data.Any()) {
                model.Avatar.ItemId = user.Id;
                model.Avatar.Type = EFile.Image;
                model.Avatar.ItemType = EItemType.UserAvatar;
                await this.fileService.CreateOrUpdate(model.Avatar!);
            }

            await this.db.Users.AddAsync(user);
            await this.db.SaveChangesAsync();

            this.logger.Information($"{nameof(Team)} - {nameof(Create)} - End", model);
            return user.Id;
        }

        public async Task ChangePassword(ChangePasswordReq request) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);
            ManagedException.ThrowIf(!PasswordHashser.Verify(request.OldPassword, user.Password), Messages.User.User_IncorrentOldPassword);

            user.Password = PasswordHashser.Hash(request.NewPassword);
            await this.db.SaveChangesAsync();
        }

        public async Task ResetPassword(string password) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == this.current.UserId);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            user.Password = PasswordHashser.Hash(password);
            await this.db.SaveChangesAsync();
        }

        public async Task UpdateMe(UserDto model) {
            this.logger.Information($"{nameof(UserService)} - {nameof(UpdateMe)} - Start", model);

            await ValidateInfo(model);
            var user = await this.db.Users.Include(o => o.GameSetting).FirstOrDefaultAsync(o => o.Id == this.current.UserId && !o.IsDelete);
            ManagedException.ThrowIf(user == null, Messages.User.User_NotFound);

            user.Name = model.Name;
            user.Phone = model.Phone;
            user.Age = model.Age;
            user.Phone = model.Phone;
            user.Sex = model.Sex;
            user.Description = model.Description;

            if (user.GameSetting != null) {
                user.GameSetting.GameTypes = JsonConvert.SerializeObject(model.GameSetting.GameTypes);
                user.GameSetting.GameTimes = JsonConvert.SerializeObject(model.GameSetting.GameTimes);
                user.GameSetting.Longitude = model.GameSetting.Longitude;
                user.GameSetting.Latitude = model.GameSetting.Latitude;
                user.GameSetting.Radius = model.GameSetting.Radius;
            } else {
                user.GameSetting = new GameSetting() {
                    Id = Guid.NewGuid().ToStringN(),
                    GameTypes = JsonConvert.SerializeObject(model.GameSetting.GameTypes),
                    GameTimes = JsonConvert.SerializeObject(model.GameSetting.GameTimes),
                    Longitude = model.GameSetting.Longitude,
                    Latitude = model.GameSetting.Latitude,
                    Radius = model.GameSetting.Radius,
                };
            }

            if (model.CoverAvatar?.Data != null && model.CoverAvatar.Data.Any()) {
                model.CoverAvatar.ItemId = user.Id;
                model.CoverAvatar.Type = EFile.Image;
                model.CoverAvatar.ItemType = EItemType.UserCover;

                var coverAvatar = await this.db.Files.FirstOrDefaultAsync(o => o.Type == EFile.Image && o.ItemType == EItemType.UserCover && o.ItemId == this.current.UserId);
                model.CoverAvatar.Id = coverAvatar?.Id;

                await this.fileService.CreateOrUpdate(model.CoverAvatar!);
            }

            if (model.Avatar?.Data != null && model.Avatar.Data.Any()) {
                model.Avatar.ItemId = user.Id;
                model.Avatar.Type = EFile.Image;
                model.Avatar.ItemType = EItemType.UserAvatar;

                var avatar = await this.db.Files.FirstOrDefaultAsync(o => o.Type == EFile.Image && o.ItemType == EItemType.UserAvatar && o.ItemId == this.current.UserId);
                model.Avatar.Id = avatar?.Id;

                await this.fileService.CreateOrUpdate(model.Avatar!);
            }

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