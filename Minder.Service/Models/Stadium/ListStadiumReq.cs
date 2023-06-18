using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Minder.Service.Models.Stadium {

    public class ListStadiumReq {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 0;
        public bool IsCount { get; set; } = false;
        public string? SearchText { get; set; }

        public List<string>? StadiumIds { get; set; }

        [JsonIgnore]
        public int Skip => PageIndex * PageSize;

        [JsonIgnore]
        public int Take => PageSize;
    }
}