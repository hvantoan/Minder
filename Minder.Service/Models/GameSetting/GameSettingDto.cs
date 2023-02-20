using Minder.Database.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Minder.Service.Models.GameSetting {

    public partial class GameSettingDto {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        public List<EGameType>? GameTypes { get; set; }
        public List<EGameTime>? GameTimes { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public double Radius { get; set; }
        public ERank Rank { get; set; }
        public int Point { get; set; }
    }

    public partial class GameSettingDto {

        public static GameSettingDto? FromEntity(Database.Models.GameSetting? entity) {
            if (entity == null) return default;

            return new GameSettingDto {
                Id = entity.Id,
                GameTypes = !string.IsNullOrEmpty(entity.GameTypes) ? JsonConvert.DeserializeObject<List<EGameType>>(entity.GameTypes) : new(),
                GameTimes = !string.IsNullOrEmpty(entity.GameTimes) ? JsonConvert.DeserializeObject<List<EGameTime>>(entity.GameTimes) : new(),
                Longitude = entity.Longitude,
                Latitude = entity.Latitude,
                Radius = entity.Radius,
                Rank = entity.Rank,
                Point = entity.Point,
            };
        }
    }
}