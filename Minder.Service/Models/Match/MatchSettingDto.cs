using Minder.Database.Enums;
using Minder.Service.Models.Stadium;
using Minder.Service.Models.Team;
using System;

namespace Minder.Service.Models.Match {

    public class MatchSettingDto {
        public string Id { get; set; } = string.Empty;
        public string? TeamId { get; set; }
        public string? StadiumId { get; set; }
        public DayOfWeek? SelectedDayOfWeek { get; set; }
        public ETime? From { get; set; }
        public ETime? To { get; set; }

        public MatchDto? HostMatch { get; set; }
        public MatchDto? OpposingMatch { get; set; }
        public StadiumDto? Stadium { get; set; }
        public TeamDto? Team { get; set; }

        public static MatchSettingDto? FromEntity(Database.Models.MatchSetting? entity) {
            if (entity == null) return default;
            return new MatchSettingDto {
                Id = entity.Id,
                TeamId = entity.TeamId,
                From = entity.From,
                To = entity.To,
                SelectedDayOfWeek = entity.SelectedDayOfWeek,
                HostMatch = MatchDto.FromEntity(entity.HostMatch!),
                Stadium = StadiumDto.FromEntity(entity.Stadium),
                Team = TeamDto.FromEntity(entity.Team),
            };
        }
    }
}