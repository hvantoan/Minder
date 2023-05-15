namespace Minder.Service.Models.Match {

    public class CreateMatchReq {
        public string HostTeamId { get; set; } = string.Empty;
        public string OpposingTeamId { get; set; } = string.Empty;
        public bool HasInvite { get; set; }
    }
}