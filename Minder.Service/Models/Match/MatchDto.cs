using Minder.Database.Enums;
using Minder.Service.Models.Common;
using Minder.Services.Resources;
using System.Collections.Generic;
using System.Linq;
using static Minder.Service.Enums;

namespace Minder.Service.Models.Match {

    public class MatchDto {
        public string Id { get; set; } = string.Empty;
        public string HostTeamId { get; set; } = string.Empty;
        public string? OpposingTeamId { get; set; }
        public EMatch Status { get; set; }
        public EDayOfWeek? SelectedDayOfWeek { get; set; }
        public ETeamSide TeamSide { get; set; }

        public MatchSettingDto? HostTeam { get; set; }
        public MatchSettingDto? OpposingTeam { get; set; }
        public List<MatchTimeOpption>? TimeChooices { get; set; }

        public static MatchDto FromEntity(Minder.Database.Models.Match entity, Minder.Database.Models.MatchSetting? hostTeam = null,
            Minder.Database.Models.MatchSetting? opposingTeam = null, string? teamId = null, List<MatchTimeOpption>? timeChooice = null, AdministrativeUnitResource? au = null) {
            return new MatchDto {
                Id = entity.Id,
                HostTeamId = entity.HostTeamId,
                OpposingTeamId = entity.OppsingTeamId,
                Status = entity.Status,
                SelectedDayOfWeek = entity.SelectedDayOfWeek,
                HostTeam = MatchSettingDto.FromEntity(hostTeam, au: au),
                OpposingTeam = MatchSettingDto.FromEntity(opposingTeam, au: au),
                TeamSide = hostTeam?.TeamId == teamId ? ETeamSide.Host : ETeamSide.Opposite,
                TimeChooices = timeChooice?.OrderBy(o => o.Date).ToList()
            };
        }
    }
}