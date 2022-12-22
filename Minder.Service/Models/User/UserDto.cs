using Minder.Services.Models.Role;
using Newtonsoft.Json;

namespace Minder.Services.Models.User {

    public partial class UserDto {
        public string? Id { get; set; }
        public string? Username { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Password { get; set; }
        public string Name { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public RoleDto? Role { get; set; }
    }

    public partial class UserDto {
        public static UserDto? FromEntity(Database.Models.User? entity, Database.Models.Role? roleEntity = null) {
            if (entity == null) return default;
            entity.Role ??= roleEntity;

            return new UserDto {
                Id = entity.Id,
                Username = entity.Username,
                Name = entity.Name, 
                IsAdmin = entity.IsAdmin,
                Role = RoleDto.FromEntity(entity.Role),
            };
        }
    }
}