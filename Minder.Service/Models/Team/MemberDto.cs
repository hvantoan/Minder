using Minder.Database.Enums;

namespace Minder.Service.Models.Team {

    public partial class MemberDto {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public ERegency Regency { get; set; }
    }

    public partial class MemberDto {

        public static MemberDto? FromEntity(Database.Models.Member? entity) {
            if (entity == null) return default;

            return new MemberDto() {
                Id = entity.Id,
                TeamId = entity.TeamId,
                UserId = entity.UserId,
                Regency = entity.Regency
            };
        }
    }
}