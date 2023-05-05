using Minder.Database.Enums;
using Minder.Services.Models;

namespace Minder.Service.Models.Team {

    public class FindTeamReq : BaseListReq {
        public int? Member { get; set; }
        public ERank? Rank { get; set; }
        public EAge? Age { get; set; }
        public EPosition? Position { get; set; }
        public EGameType? GameType { get; set; }
        public EDay? Day { get; set; }
        public ETime? Time { get; set; }
    }
}