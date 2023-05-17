using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Jobs {

    public class ExpireInvitation : IInvocable {
        private readonly MinderContext db;

        public ExpireInvitation(IServiceProvider serviceProvider) {
            this.db = serviceProvider.GetRequiredService<MinderContext>();
        }

        public async Task Invoke() {
            var invitations = await this.db.Invites.Where(o => (DateTimeOffset.Now - o.CreateAt).TotalDays > 30).ToListAsync();

            if (invitations.Any()) {
                this.db.Invites.RemoveRange(invitations);
                await this.db.SaveChangesAsync();
            }
        }
    }
}