using Minder.Exceptions;
using Minder.Service.Interfaces;
using Minder.Service.Models.AppInfo;
using Minder.Services.Common;
using Minder.Services.Resources;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class AppInfoService : BaseService, IAppInfoService {

        public AppInfoService(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task<AppVer> GetVer() {
            ManagedException.ThrowIf(!Directory.Exists("Resources"), Messages.System.System_Error);
            string filename = $"Resources/AppInfo.json";
            string body = System.IO.File.ReadAllText(filename);

            return await Task.FromResult(JsonConvert.DeserializeObject<AppVer>(body) ?? new());
        }

        public async Task<AppAbout> GetAbout() {
            ManagedException.ThrowIf(!Directory.Exists("Resources"), Messages.System.System_Error);
            string filename = $"Resources/AppInfo.json";
            string body = System.IO.File.ReadAllText(filename);

            return await Task.FromResult(JsonConvert.DeserializeObject<AppAbout>(body) ?? new());
        }

        public async Task Set(string? ver, string? aboutUs, string? hotline) {
            ManagedException.ThrowIf(!Directory.Exists("Resources"), Messages.System.System_Error);
            string filename = $"Resources/AppInfo.json";
            string body = System.IO.File.ReadAllText(filename);
            var info = JsonConvert.DeserializeObject<AppInfo>(body) ?? new();

            if (!string.IsNullOrEmpty(ver)) info.Ver = ver;
            if (!string.IsNullOrEmpty(aboutUs)) info.AboutUs = aboutUs;
            if (!string.IsNullOrEmpty(hotline)) info.Hotline = hotline;

            await File.WriteAllTextAsync(filename, JsonConvert.SerializeObject(info));
        }
    }
}