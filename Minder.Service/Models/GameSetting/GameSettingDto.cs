﻿using Minder.Database.Enums;
using Minder.Service.Models.User;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Minder.Service.Models.GameSetting {

    public partial class GameSettingDto {
        public string Id { get; set; } = string.Empty;
        public List<EGameType> GameTypes { get; set; } = new();
        public GameTime? GameTime { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public double Radius { get; set; }
        public ERank Rank { get; set; }
        public int Point { get; set; }
    }

    public partial class GameSettingDto {

        public static GameSettingDto? FromEntity(Database.Models.GameSetting entity) {
            if (entity == null) return default;

            return new GameSettingDto {
                Id = entity.Id,
                GameTypes = JsonConvert.DeserializeObject<List<EGameType>>(entity.GameTypes) ?? new(),
                GameTime = !string.IsNullOrEmpty(entity.GameTime) ? JsonConvert.DeserializeObject<GameTime>(entity.GameTime) ?? new() : new(),
                Longitude = entity.Longitude,
                Latitude = entity.Latitude,
                Radius = entity.Radius,
                Rank = entity.Rank,
                Point = entity.Point,
            };
        }
    }
}