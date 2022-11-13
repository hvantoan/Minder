namespace Minder.Services.Models.Auth {

    public class UserPermissionActionDto {
        public string Id { get; set; }
        public string ClaimName { get; set; }
        public bool IsEnable { get; set; }
        public bool IsActive { get; set; }
    }
}