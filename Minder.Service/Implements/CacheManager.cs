using Microsoft.Extensions.Caching.Memory;
using Minder.Exceptions;
using Minder.Service.Helpers;
using Minder.Service.Interfaces;
using Minder.Service.Models.Auth;
using Minder.Service.Models.CacheManager;
using Minder.Services.Resources;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class CacheManager : ICacheManager {
        private readonly IMemoryCache cache;

        public CacheManager(IMemoryCache memoryCache) {
            this.cache = memoryCache;
        }

        public string VerifyOTP(Verify verify) {
            if (cache.TryGetValue<CacheData>(verify.Username, out var data) && !string.IsNullOrWhiteSpace(verify.Code) && data!.Code == verify.Code) {
                cache.Remove(verify.Username);
                return data.Value;
            }
            throw new ManagedException(JsonConvert.SerializeObject(Messages.Auth.Auth_IncorresctOTP));
        }

        public async Task<string> CreateOrUpdate<T>(string username, T data) {
            var options = new MemoryCacheEntryOptions() {
                AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                Priority = CacheItemPriority.NeverRemove,
            };

            var otp = EMailHelper.GenarateOTP();
            if (cache.TryGetValue<string>(username, out _)) {
                cache.Remove(username);
            }

            var value = new CacheData() {
                Code = otp,
                Value = JsonConvert.SerializeObject(data)
            };

            cache.Set(username, value, options);
            return await Task.FromResult(otp);
        }
    }
}