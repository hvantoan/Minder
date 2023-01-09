using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IEmailService {

        Task<bool> SendOTP<T>(T model, string key);
    }
}