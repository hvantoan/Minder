using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Service.Models.GameSetting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minder.Service.Models.Team {

    public partial class TeamDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public ERegency? Regency { get; set; }

        public DateTimeOffset CreateAt { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public GameSettingDto? GameSetting { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<MemberDto>? Members { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Invitation>? Inviteds { get; set; }
    }

    public partial class TeamDto {

        public static TeamDto? FromEntity(Database.Models.Team? entity) {
            if (entity == null) return default;
            return new TeamDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                CreateAt = entity.CreateAt,
                GameSetting = entity.GameSetting != null ? GameSettingDto.FromEntity(entity.GameSetting) : default,
                Members = entity.Members != null ? entity.Members.Select(o => MemberDto.FromEntity(o)!).ToList() : default,
            };
        }
    }
}