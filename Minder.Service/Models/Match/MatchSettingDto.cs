using Minder.Database.Enums;
using Minder.Service.Models.Stadium;
using Minder.Service.Models.Team;
using Minder.Services.Resources;
using System;

namespace Minder.Service.Models.Match {

    public class MatchSettingDto {
        public string Id { get; set; } = string.Empty;
        public string? TeamId { get; set; }
        public string? StadiumId { get; set; }
        public EDayOfWeek? SelectedDayOfWeek { get; set; }
        public ETime? From { get; set; }
        public ETime? To { get; set; }
        public DateTimeOffset Date { get; set; }

        public MatchDto? HostMatch { get; set; }
        public MatchDto? OpposingMatch { get; set; }
        public StadiumDto? Stadium { get; set; }
        public TeamDto? Team { get; set; }

        public static MatchSettingDto? FromEntity(Database.Models.MatchSetting? entity, Database.Models.Stadium? stadium = null, AdministrativeUnitResource? au = null) {
            if (entity == null) return default;
            return new MatchSettingDto {
                Id = entity.Id,
                TeamId = entity.TeamId,
                StadiumId = entity.StadiumId,
                From = entity.From,
                To = entity.To,
                Date = entity.Date,
                SelectedDayOfWeek = entity.SelectedDayOfWeek,
                Stadium = StadiumDto.FromEntity(entity.Stadium, au),
            };
        }
    }
}