using Minder.Database.Enums;
using Minder.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minder.Service.Models.Team {

    public partial class TeamDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        //Game setting

        public List<EGameType>? GameTypes { get; set; }

        public List<EGameTime>? GameTimes { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Radius { get; set; }

        // Rank

        public ERank Rank { get; set; }
        public int Point { get; set; }
        public DateTimeOffset CreateAt { get; set; }

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
                GameTypes = !string.IsNullOrEmpty(entity.GameTypes) ? JsonConvert.DeserializeObject<List<EGameType>>(entity.GameTypes) : new(),
                GameTimes = !string.IsNullOrEmpty(entity.GameTimes) ? JsonConvert.DeserializeObject<List<EGameTime>>(entity.GameTimes) : new(),
                Longitude = entity.Longitude,
                Latitude = entity.Latitude,
                Radius = entity.Radius,
                Rank = entity.Rank,
                Point = entity.Point,
                Members = entity.Members?.Select(o => MemberDto.FromEntity(o)!).ToList() ?? new(),
                CreateAt = entity.CreateAt,
            };
        }
    }
}