using Microsoft.Extensions.Caching.Memory;
using Minder.Service.Helpers;
using Minder.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class CacheManager : ICacheManager {
        private readonly IMemoryCache cache;

        public CacheManager(IMemoryCache memoryCache) {
            this.cache = memoryCache;
        }

        public bool VerifyOtp(string username, string? code) {
            if (cache.TryGetValue(username, out var otp) && !string.IsNullOrWhiteSpace(code)) {
                var data = cache.Get<string>(username);
                return data == code;
            }
            return false;
        }

        public async Task<string> CreateOrUpdate(string username) {
            var options = new MemoryCacheEntryOptions() {
                AbsoluteExpiration = DateTime.Now.AddMinutes(3),
                Priority = CacheItemPriority.NeverRemove,
            };

            var otp = EMailHelper.GenarateOTP();
            if (cache.TryGetValue<string>(username, out _)) {
                cache.Remove(username);
            }
            cache.Set<string>(username, otp, options);
            return await Task.FromResult(otp);
        }
    }
}