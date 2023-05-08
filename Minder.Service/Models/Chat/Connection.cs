namespace Minder.Service.Models.Chat {

    public class Connection {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? GroupId { get; set; }
    }
}