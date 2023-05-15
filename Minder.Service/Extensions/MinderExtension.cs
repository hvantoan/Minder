using Minder.Service.Models.Team;
using System;
using System.Collections.Generic;

namespace Minder.Service.Extensions {

    public static class MinderExtension {

        public static List<TeamDto> SortDistance(TeamDto myTeam, List<TeamDto> teams) {
            teams.Sort((a, b) => Distance(a, myTeam).CompareTo(Distance(b, myTeam)));
            teams.ForEach(o => o.Distance = Distance(myTeam, o));
            return teams;
        }

        private static double Distance(TeamDto a, TeamDto b) {
            return Math.Sqrt((double)((a.GameSetting!.Longitude - b.GameSetting!.Longitude) * (a.GameSetting!.Longitude - b.GameSetting!.Longitude) 
                + (a.GameSetting.Latitude - b.GameSetting.Latitude) * (a.GameSetting.Latitude - b.GameSetting.Latitude)));
        }
    }
}