namespace Minder.Service.Models.Group {

    public class UpdateGroupReq {
        public string GroupId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public byte[]? Avatar { get; set; }
    }
}