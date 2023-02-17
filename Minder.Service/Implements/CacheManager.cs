using Microsoft.Extensions.Caching.Memory;
using Minder.Database.Enums;
using Minder.Exceptions;
using Minder.Service.Helpers;
using Minder.Service.Interfaces;
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

        public (string, EVerifyType) VerifyOTP(string otp) {
            if (cache.TryGetValue<CacheData>(otp, out var data) && !string.IsNullOrWhiteSpace(otp) && data!.Code == otp) {
                cache.Remove(otp);
                return (data.Value, data.Type);
            }
            throw new ManagedException(JsonConvert.SerializeObject(Messages.Auth.Auth_IncorresctOTP));
        }

        public async Task<string> CreateOrUpdate<T>(EVerifyType type, T data) {
            var options = new MemoryCacheEntryOptions() {
                AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                Priority = CacheItemPriority.NeverRemove,
            };

            string otp;
            do {
                otp = EMailHelper.GenarateOTP();
            } while (string.IsNullOrWhiteSpace(otp) || cache.TryGetValue<string>(otp, out _));

            var value = new CacheData() {
                Code = otp,
                Type = type,
                Value = JsonConvert.SerializeObject(data)
            };

            cache.Set(otp, value, options);
            return await Task.FromResult(otp);
        }
    }
}