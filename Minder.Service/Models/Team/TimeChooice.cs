using Minder.Database.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Minder.Service.Models.Team {

    public class TimeChooice {
        public EDayOfWeek Day { get; set; }

        [Description("Số lượng người có thể tham gia.")]
        public int Quantity { get; set; }

        [JsonIgnore]
        public int Length { get => Value.Count; }

        public List<int> Value { get; set; } = new List<int>();
    }
}