using Minder.Services.Models;
using System.Collections.Generic;

namespace Minder.Service.Models.User {

    public class ListUserReq : BaseListReq {
        public List<string>? UserIds { get; set; }
    }
}