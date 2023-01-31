using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class Match {
        public string Id { get; set; } = null!;
        public string? PitchId { get; set; }
        public string FirstTeamId { get; set; } = null!;
        public string SecondTeamId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public EMatch Status { get; set; }
        public int FirstTeamCore { get; set; }
        public int SecondTeamCore { get; set; }
    }
}   