using Minder.Database.Enums;
using Minder.Service.Models.Stadium;
using Minder.Service.Models.Team;
using Minder.Services.Resources;
using Newtonsoft.Json;
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

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public StadiumDto? Stadium { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public TeamDto? Team { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? TeamName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Avatar { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

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
                Stadium = entity.Stadium != null ? StadiumDto.FromEntity(entity.Stadium, au) : default,
                TeamName = entity?.Team?.Name,
                Latitude = entity?.Team?.GameSetting?.Latitude ?? decimal.Zero,
                Longitude = entity?.Team?.GameSetting?.Longitude ?? decimal.Zero,
            };
        }
    }
}