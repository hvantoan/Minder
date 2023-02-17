using Minder.Service.Models.Auth;
using Minder.Service.Models.User;
using Minder.Services.Models.Auth;
using Minder.Services.Models.User;
using System.Threading.Tasks;

namespace Minder.Services.Interfaces {

    public interface IAuthService {

        Task<LoginRes> WebLogin(LoginReq request);

        Task Register(UserDto model);

        Task ForgotPassword(ForgotPasswordReq request);

        Task<LoginRes> Refresh();

        Task<VerifyRes> Verify(VerifyUserReq request);

        Task<UserNameValidate> CheckUser(string userName);
    }
}