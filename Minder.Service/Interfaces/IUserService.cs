using Minder.Services.Models.User;
using System.Threading.Tasks;

namespace Minder.Services.Interfaces {
    public interface IUserService {
        Task ChangePassword(string oldPassword, string newPassword);
        Task<UserDto> CreateOrUpdate(UserDto model);
        Task ResetPassword(string userId, string password);
    }
}