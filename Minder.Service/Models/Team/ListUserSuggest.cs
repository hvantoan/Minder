using Newtonsoft.Json;

namespace Minder.Service.Models.Team {

    public class ListUserSuggest {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 100;
        public bool IsCount { get; set; } = false;
        public string? SearchText { get; set; }

        public string TeamId { get; set; } = string.Empty;

        [JsonIgnore]
        public int Skip => PageIndex * PageSize;

        [JsonIgnore]
        public int Take => PageSize;
    }
}