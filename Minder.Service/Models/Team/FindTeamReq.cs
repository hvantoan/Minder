using Minder.Database.Enums;
using Minder.Services.Models;
using static Minder.Service.Enums;

namespace Minder.Service.Models.Team {

    public class FindTeamReq : BaseListReq {
        public int? Member { get; set; }
        public ERank? Rank { get; set; }
        public EAge? Age { get; set; }
        public EPosition? Position { get; set; }
        public EGameType? GameType { get; set; }

        // Find in map
        public decimal Lat { get; set; }

        public decimal Long { get; set; }
        public int Radius { get; set; }

        // Find with time option
        public EDayOfWeek? Day { get; set; }

        public EGameTime From { get; set; }
        public EGameTime To { get; set; }
    }
}