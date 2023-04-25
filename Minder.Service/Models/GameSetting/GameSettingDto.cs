using Minder.Database.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Minder.Service.Models.GameSetting {

    public partial class GameSettingDto {
        public string Id { get; set; } = string.Empty;
        public List<EGameType> GameTypes { get; set; } = new();
        public GameTime? GameTime { get; set; }
        public List<EPosition> Positions { get; set; } = new();
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public double Radius { get; set; }
        public ERank Rank { get; set; }

        [JsonIgnore]
        public int Point { get; set; }
    }

    public partial class GameSettingDto {

        public static GameSettingDto? FromEntity(Database.Models.GameSetting entity) {
            if (entity == null) return default;

            return new GameSettingDto {
                Id = entity.Id,
                GameTypes = JsonConvert.DeserializeObject<List<EGameType>>(entity.GameTypes) ?? new(),
                GameTime = !string.IsNullOrEmpty(entity.GameTime) ? JsonConvert.DeserializeObject<GameTime>(entity.GameTime) ?? new() : new(),
                Positions = JsonConvert.DeserializeObject<List<EPosition>>(entity.Positions) ?? new(),
                Longitude = entity.Longitude,
                Latitude = entity.Latitude,
                Radius = entity.Radius,
                Rank = entity.Rank,
            };
        }
    }
}