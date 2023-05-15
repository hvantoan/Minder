using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Jobs {

    public class ExpireTeamRejected : IInvocable {
        private readonly MinderContext db;

        public ExpireTeamRejected(IServiceProvider serviceProvider) {
            this.db = serviceProvider.GetRequiredService<MinderContext>();
        }

        public async Task Invoke() {
            // Has One day
            //var periousDay = DateTimeOffset.UtcNow.AddDays(-1);

            
            var periousDay = DateTimeOffset.UtcNow.AddMinutes(-10);
            var teamRejecteds = await this.db.TeamRejecteds.Where(o => o.CreateAt >= periousDay).ToListAsync();

            this.db.RemoveRange(teamRejecteds);
            await this.db.SaveChangesAsync();
        }
    }
}