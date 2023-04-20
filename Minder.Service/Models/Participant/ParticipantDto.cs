using Minder.Extensions;
using Minder.Service.Models.Conversation;
using Minder.Services.Models.User;
using Newtonsoft.Json;
using System;

namespace Minder.Service.Models.Participant {

    public class ParticipantDto {
        public string Id { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTimeOffset JoinAt { get; set; } = DateTimeOffset.Now;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public UserDto? User { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public ConversationDto? Conversation { get; set; }

        public Database.Models.Participant ToEntity() {
            return new() {
                Id = !string.IsNullOrEmpty(Id) ? Id : Guid.NewGuid().ToStringN(),
                ConversationId = ConversationId,
                UserId = UserId,
                JoinAt = JoinAt,
            };
        }
    }
}