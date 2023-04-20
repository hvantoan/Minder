using Minder.Extensions;
using Minder.Service.Models.Message;
using Minder.Service.Models.Participant;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minder.Service.Models.Conversation
{

    public class ConversationDto {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.Now;
        public bool Online { get; set; }

        [JsonIgnore]
        public List<string>? ParticipantIds { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageDto>? Messages { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<ParticipantDto>? Participants { get; set; }

        public Database.Models.Conversation ToEntity() {
            return new() {
                Id = !string.IsNullOrEmpty(this.Id) ? this.Id : Guid.NewGuid().ToStringN(),
                Title = this.Title,
                ChannelId = this.ChannelId ?? string.Empty,
                CreateAt = this.CreateAt,
                Participants = this.Participants != null ? this.Participants.Select(o => o.ToEntity()).ToList() : default,
            };
        }

        public static ConversationDto? FromEntity(Database.Models.Conversation? entity) {
            if (entity == null) return default;
            return new ConversationDto() {
                Id = entity.Id,
                Title = entity.Title,
                ChannelId = entity.ChannelId,
                CreateAt = entity.CreateAt,
            };
        }
    }
}