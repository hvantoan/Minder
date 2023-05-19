using Minder.Database.Enums;
using System;
using static Minder.Service.Enums;

namespace Minder.Service.Models.Match {

    public class MatchDto {
        public string Id { get; set; } = string.Empty;
        public string HostTeamId { get; set; } = string.Empty;
        public string? OpposingTeamId { get; set; }
        public EMatch Status { get; set; }
        public DayOfWeek? SelectedDate { get; set; }
        public ETeamSide TeamSide { get; set; }

        public MatchSettingDto? HostTeam { get; set; }
        public MatchSettingDto? OpposingTeam { get; set; }

        public static MatchDto? FromEntity(Minder.Database.Models.Match entity, Minder.Database.Models.MatchSetting? hostTeam = null,
            Minder.Database.Models.MatchSetting? opposingTeam = null, string? teamId = null) {
            if (entity == null) return default;
            return new MatchDto {
                Id = entity.Id,
                HostTeamId = entity.HostTeamId,
                OpposingTeamId = entity.OppsingTeamId,
                SelectedDate = entity.SelectedDate,
                HostTeam = MatchSettingDto.FromEntity(hostTeam),
                OpposingTeam = MatchSettingDto.FromEntity(opposingTeam),
                TeamSide = hostTeam?.TeamId == teamId ? ETeamSide.Host : ETeamSide.Opposite
            };
        }
    }
}