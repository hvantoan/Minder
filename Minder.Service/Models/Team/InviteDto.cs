using Minder.Database.Enums;
using System;

namespace Minder.Service.Models.Team {

    public partial class InviteDto {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public EInvitationType Type { get; set; } = EInvitationType.Invite;
        public DateTimeOffset? CreateAt { get; set; }
    }

    public partial class InviteDto {

        public static InviteDto? FromEntity(Database.Models.Invitation? entity) {
            if (entity == null) return default;
            return new InviteDto {
                Id = entity.Id,
                TeamId = entity.TeamId,
                UserId = entity.UserId,
                Description = entity.Description,
                Type = entity.Type,
                CreateAt = entity.CreateAt,
            };
        }
    }
}