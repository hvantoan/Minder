using Minder.Database.Enums;
using Minder.Extensions;
using System;

namespace Minder.Service.Models.Message {

    public class MessageDto {
        public string Id { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public EMessageType MessageType { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.Now;

        public static MessageDto? FromEntity(Database.Models.Message? entity) {
            if (entity == null) return default;

            return new MessageDto {
                Id = entity.Id,
                ConversationId = entity.ConversationId,
                SenderId = entity.SenderId,
                MessageType = entity.MessageType,
                Content = entity.Content,
                CreateAt = entity.CreateAt
            };
        }

        public Database.Models.Message ToEntity() {
            return new Database.Models.Message {
                Id = string.IsNullOrWhiteSpace(this.Id) ? Guid.NewGuid().ToStringN() : this.Id,
                ConversationId = this.ConversationId,
                SenderId = this.SenderId,
                MessageType = this.MessageType,
                Content = this.Content,
                CreateAt = this.CreateAt,
            };
        }
    }
}