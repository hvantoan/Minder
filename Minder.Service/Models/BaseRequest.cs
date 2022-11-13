namespace Minder.Services.Models {

    public class BaseGetRequest {
        public string Id { get; set; }
    }

    public class BaseListRequest {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool IsCount { get; set; }
        public string SearchText { get; set; }
    }
}