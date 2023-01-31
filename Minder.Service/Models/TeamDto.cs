using Minder.Database.Enums;
using Minder.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Minder.Service.Models {

    public partial class TeamDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        //Game setting
        public List<EGameType> GameType { get; set; } = new();

        public string GameTime { get; set; } = string.Empty;
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Radius { get; set; }

        // Rank
        public ERank Rank { get; set; }

        public int Point { get; set; }

        public DateTimeOffset CreateAt { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Member>? Members { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Invited>? Inviteds { get; set; }
    }

    public partial class TeamDto {

        public static TeamDto? FromEntity(Database.Models.Team? entity) {
            if (entity == null) return default;
            return new TeamDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                GameType = JsonConvert.DeserializeObject<List<EGameType>>(entity.GameType) ?? new(),
                GameTime = entity.GameTime,
                Longitude = entity.Longitude,
                Latitude = entity.Latitude,
                Radius = entity.Radius,
                Rank = entity.Rank,
                Point = entity.Point,
                CreateAt = entity.CreateAt,
            };
        }
    }
}