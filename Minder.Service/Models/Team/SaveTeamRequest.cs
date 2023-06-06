namespace Minder.Service.Models.Team {

    public class SaveTeamRequest : TeamDto {
        public byte[]? AvatarData { get; set; }
        public byte[]? CoverData { get; set; }
    }
}