using Minder.Database.Enums;

namespace Minder.Service.Models.Auth {

    public class Verify {
        public string Code { get; set; } = null!;
        public string Username { get; set; } = null!;
        public EVerifyType Type { get; set; }
    }
}