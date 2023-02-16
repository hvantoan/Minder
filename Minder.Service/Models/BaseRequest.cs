namespace Minder.Services.Models {

    public class BaseReq {
        public string Id { get; set; } = string.Empty;
    }

    public class BaseListReq {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public bool IsCount { get; set; } = false;
        public string? SearchText { get; set; }
    }

    public class BaseFileReq {
        public string FileName { get; set; } = string.Empty;
        public byte[]? Data { get; set; }
    }
}