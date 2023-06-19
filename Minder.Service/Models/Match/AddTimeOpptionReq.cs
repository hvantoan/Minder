using System;

namespace Minder.Service.Models.Match {

    public class AddTimeOpptionReq {
        public string MatchId { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}