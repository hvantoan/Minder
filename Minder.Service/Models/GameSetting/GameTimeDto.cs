using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Minder.Service.Models.GameSetting {

    public class GameTimeDto {
        public string? Id { get; set; }
        public List<EGameTime> Monday { get; set; } = new();
        public List<EGameTime> Tuesday { get; set; } = new();
        public List<EGameTime> Wednesday { get; set; } = new();
        public List<EGameTime> Thursday { get; set; } = new();
        public List<EGameTime> Friday { get; set; } = new();
        public List<EGameTime> Saturday { get; set; } = new();
        public List<EGameTime> Sunday { get; set; } = new();

        public static GameTimeDto FromEntity(GameTime times) {
            return new GameTimeDto() {
                Id = times.Id,
                Monday = JsonConvert.DeserializeObject<List<EGameTime>>(times.Monday) ?? new(),
                Tuesday = JsonConvert.DeserializeObject<List<EGameTime>>(times.Tuesday) ?? new(),
                Thursday = JsonConvert.DeserializeObject<List<EGameTime>>(times.Thursday) ?? new(),
                Wednesday = JsonConvert.DeserializeObject<List<EGameTime>>(times.Wednesday) ?? new(),
                Friday = JsonConvert.DeserializeObject<List<EGameTime>>(times.Friday) ?? new(),
                Saturday = JsonConvert.DeserializeObject<List<EGameTime>>(times.Saturday) ?? new(),
                Sunday = JsonConvert.DeserializeObject<List<EGameTime>>(times.Sunday) ?? new(),
            };
        }

        public GameTime ToEntity() {
            return new GameTime() {
                Id = this.Id ?? Guid.NewGuid().ToStringN(),
                Monday = JsonConvert.SerializeObject(this.Monday),
                Tuesday = JsonConvert.SerializeObject(this.Tuesday),
                Thursday = JsonConvert.SerializeObject(this.Thursday),
                Wednesday = JsonConvert.SerializeObject(this.Wednesday),
                Friday = JsonConvert.SerializeObject(this.Friday),
                Saturday = JsonConvert.SerializeObject(this.Saturday),
                Sunday = JsonConvert.SerializeObject(this.Sunday),
            };
        }
    }
}