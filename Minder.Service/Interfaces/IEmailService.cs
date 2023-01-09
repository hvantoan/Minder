using Minder.Services.Models.User;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IEmailService {

        Task<bool> SendOTP(UserDto user);
    }
}