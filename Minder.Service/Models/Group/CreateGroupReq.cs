using System.Collections.Generic;

namespace Minder.Service.Models.Group {

    public class CreateGroupReq {
        public string Title { get; set; } = string.Empty;
        public List<string> userIds { get; set; } = new();
    }
}