using Minder.Database.Enums;
using Minder.Service.Models.File;
using Minder.Service.Models.GameSetting;
using Minder.Services.Models.Role;
using Newtonsoft.Json;

namespace Minder.Services.Models.User {

    public partial class UserDto {
        public string? Id { get; set; }
        public string? Username { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Password { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public ESex Sex { get; set; } = ESex.Unknown;
        public int Age { get; set; }
        public string? Description { get; set; }
        public GameSettingDto GameSetting { get; set; } = new();

        public bool IsAdmin { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public FileDto? Avatar { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public FileDto? CoverAvatar { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public RoleDto? Role { get; set; }
    }

    public partial class UserDto {

        public static UserDto? FromEntity(Database.Models.User? entity, Database.Models.Role? roleEntity = null, FileDto? avatar = null, FileDto? coverAvatar = null) {
            if (entity == null) return default;
            entity.Role ??= roleEntity;

            return new UserDto {
                Id = entity.Id,
                Username = entity.Username,
                Name = entity.Name,
                Phone = entity.Phone,
                Age = entity.Age,
                Description = entity.Description,
                Avatar = avatar,
                CoverAvatar = coverAvatar,
                IsAdmin = entity.IsAdmin,
                Role = RoleDto.FromEntity(entity.Role),
            };
        }
    }
}