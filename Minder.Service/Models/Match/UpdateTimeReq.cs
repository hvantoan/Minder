using Minder.Database.Enums;

namespace Minder.Service.Models.Match {

    public class UpdateTimeReq {
        public string TeamId { get; set; } = string.Empty;

        public EDayOfWeek DayOfWeek { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}