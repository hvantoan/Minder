using System.Collections.Generic;

namespace Minder.Service.Models.Team {

    public class TimeChooice {
        public int Quantity { get; set; }
        public int Length { get => Value.Count; }
        public List<int> Value { get; set; } = new List<int>();
    }
}