using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database;
using Minder.Service.Models;
using Minder.Services.Common.Lock;
using System;
using System.Threading.Tasks;
using TuanVu.Services.Common;

namespace Minder.Services.Common {

    public class BaseService {
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly IServiceProvider serviceProvider;
        protected readonly IConfiguration configuration;
        protected readonly CurrentUser current;
        protected readonly MinderContext db;
        protected readonly ILogger logger;

        protected BaseService(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
            this.httpContextAccessor = this.serviceProvider.GetRequiredService<IHttpContextAccessor>();
            this.configuration = this.serviceProvider.GetRequiredService<IConfiguration>();
            this.current = this.serviceProvider.GetRequiredService<CurrentUser>();
            this.db = this.serviceProvider.GetRequiredService<MinderContext>();
            this.logger = Logger.Create("User", this.current.UserId);
        }

        protected static async Task LockActionByKey(string key, Func<Task> action, int expirySec = 60) {
            using var locker = await new Locker(key, expirySec).Lock();
            await action.Invoke();
        }

        protected static async Task<T> LockActionByKey<T>(string key, Func<Task<T>> action, int expirySec = 60) {
            using var locker = await new Locker(key, expirySec).Lock();
            return await action.Invoke();
        }
    }
}