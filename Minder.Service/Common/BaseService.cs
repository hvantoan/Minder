using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Minder.Database;

namespace Minder.Services.Common {

    public class BaseService {
        protected readonly MinderContext db;
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly string currentUserId;
        protected readonly string currentUrl;
        protected readonly IConfiguration configuration;

        protected BaseService(MinderContext db, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) {
            this.db = db;
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;

            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext != null) {
                this.currentUserId = httpContext.User?.FindFirst(o => o.Type == "UserId")?.Value ?? "";
                this.currentUrl = this.GetCurrentUrl(httpContext.Request);
            }
        }
        protected BaseService(MinderContext db, IHttpContextAccessor httpContextAccessor) {
            this.db = db;
            this.httpContextAccessor = httpContextAccessor;

            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext != null) {
                this.currentUserId = httpContext.User?.FindFirst(o => o.Type == "UserId")?.Value ?? "";
                this.currentUrl = this.GetCurrentUrl(httpContext.Request);
            }
        }

        private string GetCurrentUrl(HttpRequest httpRequest) {
            if (httpRequest == null) return default;
            return $"{httpRequest.Scheme}://{httpRequest.Host.Value}/";
        }
    }
}