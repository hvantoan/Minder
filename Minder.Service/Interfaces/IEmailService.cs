using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IEmailService {

        Task<bool> SendOTP(string otp, string toEmailAddress);
    }
}