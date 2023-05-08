using Newtonsoft.Json;

namespace Minder.Service.Models.User {

    public class UserMessage {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Avatar { get; set; }

        public static UserMessage FromEntity(Minder.Database.Models.User user, string avatar) {
            return new UserMessage { Id = user.Id, Name = user.Name, Avatar = avatar };
        }
    }
}