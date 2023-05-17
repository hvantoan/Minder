using Minder.Services.Models;

namespace Minder.Service.Models.Match {

    public class ListMatchReq : BaseListReq {
        public string TeamId { get; set; } = string.Empty;
    }
}