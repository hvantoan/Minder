namespace Minder.Service.Models.Team {

    public class CreateInviteSwipeCardReq {
        public string TeamId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool HasInvite { get; set; }
    }
}