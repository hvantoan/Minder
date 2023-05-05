using Minder.Services.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Minder.Service.Models.Team {

    public class ListTeamReq : BaseListReq {
        public bool IsMyTeam { get; set; }

        [JsonIgnore]
        public List<string>? teamIds { get; set; }
    }
}