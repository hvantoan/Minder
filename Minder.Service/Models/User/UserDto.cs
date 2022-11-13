using Newtonsoft.Json;

namespace Minder.Services.Models.User {

    public partial class UserDto {
        public string Id { get; set; }
        public string Username { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }

        public string RoleId { get; set; }

    }

    public partial class UserDto {
        public static UserDto FromEntity(Database.Models.User entity) {
            if (entity == null) return default;

            return new UserDto {
                Id = entity.Id,
                Username = entity.Username,
                IsActive = entity.IsActive,
                IsAdmin = entity.IsAdmin,
                RoleId = entity.RoleId,
            };
        }
    }
}