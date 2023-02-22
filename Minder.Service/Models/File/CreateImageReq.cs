using Minder.Database.Enums;

namespace Minder.Service.Models.File {

    public class CreateImageReq {
        public string ItemId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public EItemType ItemType { get; set; } = EItemType.UserAvatar;
        public byte[]? Data { get; set; }
    }
}