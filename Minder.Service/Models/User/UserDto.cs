using Minder.Database.Enums;
using Minder.Service.Models.GameSetting;
using Minder.Services.Models.Role;
using Newtonsoft.Json;
using System;

namespace Minder.Services.Models.User {

    public partial class UserDto {
        public string Id { get; set; } = string.Empty;
        public string? Username { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Password { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public ESex Sex { get; set; } = ESex.Unknown;
        public DateTimeOffset DayOfBirth { get; set; }
        public string? Description { get; set; }
        public GameSettingDto? GameSetting { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Avatar { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Cover { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public RoleDto? Role { get; set; }

        [JsonIgnore]
        public double Distance { get; set; }
    }

    public partial class UserDto {

        public static UserDto? FromEntity(Database.Models.User? entity, Database.Models.Role? roleEntity = null, string? avatar = null, string? cover = null) {
            if (entity == null) return default;
            entity.Role ??= roleEntity;

            return new UserDto {
                Id = entity.Id,
                Username = entity.Username,
                Name = entity.Name,
                Phone = entity.Phone,
                DayOfBirth = entity.DayOfBirth,
                Sex = entity.Sex,
                Description = entity.Description,
                Avatar = avatar,
                Cover = cover,
                Role = RoleDto.FromEntity(entity.Role),
                GameSetting = GameSettingDto.FromEntity(entity.GameSetting ?? new()) ?? new(),
            };
        }
    }
}