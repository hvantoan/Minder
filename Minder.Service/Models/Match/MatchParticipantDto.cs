namespace Minder.Service.Models.Match {

    public class MatchParticipantDto {
        public string UserId { get; set; } = string.Empty;
        public string MatchId { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
    }
}