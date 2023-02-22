using Minder.Database.Enums;
using System.Collections.Generic;

namespace Minder.Service.Models.User {

    public class GameTime {
        public List<EGameTime> Monday { get; set; } = new();
        public List<EGameTime> Tuesday { get; set; } = new();
        public List<EGameTime> Wednesday { get; set; } = new();
        public List<EGameTime> Thursday { get; set; } = new();
        public List<EGameTime> Friday { get; set; } = new();
        public List<EGameTime> Saturday { get; set; } = new();
        public List<EGameTime> Sunday { get; set; } = new();
    }
}