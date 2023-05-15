using Minder.Database.Enums;
using System;

namespace Minder.Service.Models.Match {

    public class MatchDto {
        public string Id { get; set; } = string.Empty;
        public string HostTeamId { get; set; } = string.Empty;
        public string? OpposingTeamId { get; set; }
        public EMatch Status { get; set; }
        public DayOfWeek? SelectedDate { get; set; }

        public MatchSettingDto? HostTeam { get; set; }
        public MatchSettingDto? OpposingTeam { get; set; }

        public static MatchDto? FromEntity(Minder.Database.Models.Match entity) {
            if (entity == null) return default;
            return new MatchDto {
                Id = entity.Id,
                HostTeamId = entity.HostTeamId,
                OpposingTeamId = entity.OppsingTeamId,
                SelectedDate = entity.SelectedDate,
                HostTeam = MatchSettingDto.FromEntity(entity.HostTeam),
                OpposingTeam = MatchSettingDto.FromEntity(entity.OpposingTeam),
            };
        }
    }
}