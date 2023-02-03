using Minder.Database.Enums;
using Minder.Service.Models.File;
using Minder.Services.Models.Role;
using Newtonsoft.Json;
using System.Collections.Generic;

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

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public FileDto? Avatar { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public FileDto? CoverAvatar { get; set; }

        public List<EGameType>? GameTypes { get; set; }
        public List<EGameTime>? GameTimes { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Radius { get; set; }

        public ERank Rank { get; set; }
        public int Point { get; set; }

        public bool IsAdmin { get; set; }

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
                GameTypes = !string.IsNullOrEmpty(entity.GameTypes) ? JsonConvert.DeserializeObject<List<EGameType>>(entity.GameTypes) : new(),
                GameTimes = !string.IsNullOrEmpty(entity.GameTimes) ? JsonConvert.DeserializeObject<List<EGameTime>>(entity.GameTimes) : new(),
                Longitude = entity.Longitude,
                Latitude = entity.Latitude,
                Radius = entity.Radius,
                Rank = entity.Rank,
                Point = entity.Point,
                IsAdmin = entity.IsAdmin,
                Role = RoleDto.FromEntity(entity.Role),
            };
        }
    }
}