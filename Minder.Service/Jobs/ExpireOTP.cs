using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Jobs {

    public class ExpireOTP : IInvocable {
        private readonly MinderContext db;

        public ExpireOTP(IServiceProvider serviceProvider) {
            this.db = serviceProvider.GetRequiredService<MinderContext>();
        }

        public async Task Invoke() {
            var periousDay = DateTimeOffset.UtcNow.AddMinutes(-5);
            var otps = await this.db.RegistrationInformations.Where(o => o.CreateAt < periousDay).ToListAsync();

            if (otps.Any()) {
                this.db.RegistrationInformations.RemoveRange(otps);
                await this.db.SaveChangesAsync();
            }
        }
    }
}