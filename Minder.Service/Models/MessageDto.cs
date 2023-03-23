using Minder.Database.Enums;
using Minder.Service.Models.Conversation;
using Minder.Services.Models.User;
using Newtonsoft.Json;
using System;

namespace Minder.Service.Models {

    public class MessageDto {
        public string Id { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public EMessageType MessageType { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreateAt { get; set; }


        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public ConversationDto? Conversation { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public UserDto? User { get; set; }
    }
}