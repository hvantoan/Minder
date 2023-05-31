using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Service.Models.GameSetting;
using Minder.Services.Models.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minder.Service.Models.Team {

    public partial class TeamDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsAutoLocation { get; set; } = false;
        public bool IsAutoTime { get; set; } = false;
        public ERank Rank { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? GroupId { get; set; } = string.Empty;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public ERegency? Regency { get; set; }

        public DateTimeOffset CreateAt { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public GameSettingDto? GameSetting { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<MemberDto>? Members { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Invitation>? Inviteds { get; set; }

        [JsonIgnore]
        public virtual List<string> TeamRejectedId { get; set; } = new List<string>();

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Avatar { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Cover { get; set; }

        public double Distance { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Owner { get; set; }
    }

    public partial class TeamDto {

        public static TeamDto FromEntity(Database.Models.Team entity,string? avatar = null, string? cover = null, string? groupId = null) {
            return new TeamDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                CreateAt = entity.CreateAt,
                GameSetting = entity.GameSetting != null ? GameSettingDto.FromEntity(entity.GameSetting) : default,
                Members = entity.Members != null ? entity.Members.Select(o => MemberDto.FromEntity(o)!).ToList() : default,
                Rank = entity.GameSetting?.Rank ?? default,
                TeamRejectedId = entity.TeamRejecteds?.Where(o => o.Type == ETeamRejected.Team).Select(o => o.ItemId).ToList() ?? new List<string>(),
                GroupId = groupId,
                Avatar = avatar,
                Cover = cover,
                IsAutoTime = entity.IsAutoTime,
                IsAutoLocation = entity.IsAutoLocation,
            };
        }
    }
}