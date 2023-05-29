using Minder.Database.Enums;
using Minder.Extensions;
using Minder.Service.Models.User;
using System;

namespace Minder.Service.Models.Message {

    public class MessageDto {
        public string Id { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public EMessageType MessageType { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.Now;
        public bool IsSend { get; set; }
        public bool IsDisplayAvatar { get; set; }
        public bool IsDisplayTime { get; set; }
        public UserMessage? User { get; set; }

        public static MessageDto FromEntity(Database.Models.Message entity, string conectionUserId, Database.Models.File? avatar, string? imageUrl) {
            return new MessageDto {
                Id = entity.Id,
                GroupId = entity.GroupId,
                SenderId = entity.SenderId,
                MessageType = entity.MessageType,
                Content = entity.Content,
                CreateAt = entity.CreateAt,
                IsSend = conectionUserId == entity.SenderId,
                User = new UserMessage() {
                    Id = entity.User?.Id ?? "",
                    Name = entity.User?.Name ?? "",
                    Avatar = !string.IsNullOrEmpty(avatar?.Path) ? $"{imageUrl}/{avatar.Path}" : default
                }
            };
        }

        public Database.Models.Message ToEntity() {
            return new Database.Models.Message {
                Id = string.IsNullOrWhiteSpace(this.Id) ? Guid.NewGuid().ToStringN() : this.Id,
                GroupId = this.GroupId,
                SenderId = this.SenderId,
                MessageType = this.MessageType,
                Content = this.Content,
                CreateAt = this.CreateAt.ToUniversalTime(),
                UpdateAt = DateTimeOffset.Now,
            };
        }
    }
}