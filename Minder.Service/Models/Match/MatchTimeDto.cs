using Minder.Database.Enums;
using System;

namespace Minder.Services.Models.Match {

    public class MatchTimeDto {
        public string DisplayName { get; set; } = string.Empty;
        // Time

        public DayOfWeek DayOfWeek { get; set; }
        public DateTimeOffset Date { get; set; }
        public EGameTime From { get; set; }
        public EGameTime To { get; set; }

        // Total member
        public int Quantity { get; set; }
    }
}