using Minder.Service.Models.Team;
using Minder.Services.Models.User;
using System;
using System.Collections.Generic;

namespace Minder.Service.Extensions {

    public static class MinderExtension {

        public static List<TeamDto> SortDistance(TeamDto myTeam, List<TeamDto> teams) {
            teams.Sort((a, b) => Distance(a, myTeam).CompareTo(Distance(b, myTeam)));
            teams.ForEach(o => o.Distance = Distance(myTeam, o));
            return teams;
        }

        public static List<UserDto> SortUserDistance(TeamDto myTeam, List<UserDto> users) {
            users.Sort((a, b) => Distance(myTeam, a).CompareTo(Distance(myTeam, b)));
            users.ForEach(o => o.Distance = Distance(myTeam, o));
            return users;
        }

        private static double Distance(TeamDto a, UserDto b) {
            return Math.Sqrt((double)((a.GameSetting!.Longitude - b.GameSetting!.Longitude) * (a.GameSetting!.Longitude - b.GameSetting!.Longitude)
                + (a.GameSetting.Latitude - b.GameSetting.Latitude) * (a.GameSetting.Latitude - b.GameSetting.Latitude)));
        }

        private static double Distance(TeamDto a, TeamDto b) {
            return Math.Sqrt((double)((a.GameSetting!.Longitude - b.GameSetting!.Longitude) * (a.GameSetting!.Longitude - b.GameSetting!.Longitude)
                + (a.GameSetting.Latitude - b.GameSetting.Latitude) * (a.GameSetting.Latitude - b.GameSetting.Latitude)));
        }
    }
}