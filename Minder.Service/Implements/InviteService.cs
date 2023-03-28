using Microsoft.EntityFrameworkCore;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.Team;
using Minder.Services.Common;
using Minder.Services.Extensions;
using Minder.Services.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Minder.Services.Resources.Messages;

namespace Minder.Service.Implements {

    public class InviteService : BaseService, IInviteSevice {

        public InviteService(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task Create(InviteDto model) {
            var invitation = await this.db.Invites.AnyAsync(o => o.TeamId == model.TeamId && o.UserId == model.UserId);
            ManagedException.ThrowIf(invitation, Messages.Invite.Invite_IsExited);

            var team = await this.db.Teams.FirstOrDefaultAsync(o => o.Id == model.TeamId);
            ManagedException.ThrowIf(team == null, Messages.Invite.Invite_TeamNotFound);
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == model.UserId);
            ManagedException.ThrowIf(user == null, Messages.Invite.Invite_UserNotFound);

            var invite = new Invitation() {
                Id = Guid.NewGuid().ToStringN(),
                Description = model.Description,
                TeamId = team.Id,
                UserId = user.Id,
                Type = model.Type,
                CreateAt = DateTimeOffset.Now,
            };

            await this.db.Invites.AddAsync(invite);
            await this.db.SaveChangesAsync();
        }

        public async Task<ListInviteRes> ListInvite(ListInviteReq req) {
            var query = this.db.Invites
                    .WhereIf(string.IsNullOrEmpty(req.TeamId), o => o.Type == EInvitationType.Invite && o.UserId == this.current.UserId)
                    .WhereIf(!string.IsNullOrEmpty(req.TeamId), o => o.Type == EInvitationType.Invited && o.TeamId == req.TeamId);

            if (!string.IsNullOrWhiteSpace(req.SearchText)) {
                req.SearchText = req.SearchText.ReplaceSpace(true);
                query = query.Where(o => o.User!.Name.ReplaceSpace(true).Contains(req.SearchText));
            }

            return new() {
                Count = await query.CountIf(req.IsCount, o => o.Id),
                Items = await query.OrderBy(o => o.CreateAt).Skip(req.PageIndex * req.PageSize).Take(req.PageSize).Select(o => InviteDto.FromEntity(o)).ToListAsync(),
            };
        }

        public async Task ConfirmInvite(ConfirmInviteReq req) {
            var invite = await this.db.Invites.FirstOrDefaultAsync(o => o.UserId == this.current.UserId && o.Id == req.Id);
            ManagedException.ThrowIf(invite == null, Messages.Invite.Invite_NotFound);

            if (req.IsJoin) {
                var member = new Member() {
                    Id = Guid.NewGuid().ToStringN(),
                    Regency = ERegency.Member,
                    TeamId = invite.TeamId,
                    UserId = this.current.UserId,
                };

                await this.db.Members.AddAsync(member);
            }

            this.db.Invites.Remove(invite);
            await this.db.SaveChangesAsync();
        }
    }
}