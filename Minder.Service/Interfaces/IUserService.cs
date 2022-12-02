using Minder.Database.Enums;
using Minder.Services.Models.User;
using System.Threading.Tasks;

namespace Minder.Services.Interfaces {
    public interface IUserService {
        Task ChangePassword(string oldPassword, string newPassword);
        Task Create(UserDto model, ERoleType Role);
        Task Update(UserDto model);
        Task ResetPassword(string password);
        Task<UserDto> Get();
    }
}