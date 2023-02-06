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

        public async Task<AppInfo> Get() {
            ManagedException.ThrowIf(!Directory.Exists("Resources"), Messages.System.System_Error);
            string filename = $"Resources/AppInfo.json";
            string body = System.IO.File.ReadAllText(filename);

            return await Task.FromResult(JsonConvert.DeserializeObject<AppInfo>(body) ?? new());
        }

        public async Task Set(string ver) {
            ManagedException.ThrowIf(!Directory.Exists("Resources"), Messages.System.System_Error);
            string filename = $"Resources/AppInfo.json";
            await File.WriteAllTextAsync(filename, JsonConvert.SerializeObject(new AppInfo() { Ver = ver }));
        }
    }
}