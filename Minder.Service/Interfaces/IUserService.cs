using Minder.Service.Models.User;
using Minder.Services.Models.User;
using System.Threading.Tasks;

namespace Minder.Services.Interfaces {

    public interface IUserService {

        Task<UserDto?> Get(string? key);

        Task<string> Create(UserDto model);

        Task UpdateMe(UserDto model);

        Task Delete();

        Task ChangePassword(ChangePasswordRequest request);

        Task ResetPassword(ForgotPasswordRequest request);
    }
}