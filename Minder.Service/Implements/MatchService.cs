using Microsoft.EntityFrameworkCore;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.Match;
using Minder.Services.Common;
using Minder.Services.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Minder.Service.Enums;

namespace Minder.Service.Implements {

    public class MatchService : BaseService, IMatchService {

        public MatchService(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task SwipeCard(CreateMatchReq req) {
            var teams = await this.db.Teams.Where(o => o.Id == req.OpposingTeamId || o.Id == req.HostTeamId).ToListAsync();
            ManagedException.ThrowIf(!teams.Any(o => o.Id == req.HostTeamId), Messages.Match.HostTeam_NotFount);
            ManagedException.ThrowIf(!teams.Any(o => o.Id == req.OpposingTeamId), Messages.Match.HostTeam_NotFount);
            if (req.HasInvite) {
                await Create(req);
            } else {
                var item = new TeamRejected() {
                    TeamId = req.HostTeamId,
                    ItemId = req.OpposingTeamId,
                };
                await this.db.TeamRejecteds.AddAsync(item);
                await this.db.SaveChangesAsync();
            }
        }

        public async Task<string> Create(CreateMatchReq req) {
            var teams = await this.db.Teams.Where(o => o.Id == req.HostTeamId || o.Id == req.OpposingTeamId).ToListAsync();

            ManagedException.ThrowIf(string.IsNullOrEmpty(req.HostTeamId) && teams.Count > 1, Messages.Match.HostTeam_NotFount);
            ManagedException.ThrowIf(string.IsNullOrEmpty(req.OpposingTeamId) && teams.Count > 1, Messages.Match.OpposingTeam_NotFount);

            var match = await this.db.Matches.FirstOrDefaultAsync(o => o.Status == EMatch.WaitingConfirm && o.OpposingTeam!.TeamId == req.OpposingTeamId);
            if (match == null) {
                match = new Match() {
                    Id = Guid.NewGuid().ToStringN(),
                    Status = EMatch.WaitingConfirm,
                    HostTeam = new MatchSetting() {
                        Id = Guid.NewGuid().ToStringN(),
                        TeamId = req.HostTeamId,
                    },
                    OpposingTeam = new MatchSetting() {
                        Id = Guid.NewGuid().ToStringN(),
                        TeamId = req.OpposingTeamId,
                    }
                };
                await this.db.Matches.AddAsync(match);
            } else {
                match.Status = EMatch.Prepare;
            }
            await this.db.SaveChangesAsync();
            return match.Id;
        }

        public async Task<ListMatchRes> List(ListMatchReq req) {
            ManagedException.ThrowIf(string.IsNullOrEmpty(req.TeamId), Messages.Match.Match_TeamIdRequire);

            var teamExit = await this.db.Teams.AnyAsync(o => o.Id == req.TeamId);
            ManagedException.ThrowIf(!teamExit, Messages.Team.Team_NotFound);

            var query = this.db.Matches.AsNoTracking().Where(o => o.HostTeam!.TeamId == req.TeamId || o.OpposingTeam!.TeamId == req.TeamId);
            var hostTeamId = await query.Select(o => o.HostTeamId).ToListAsync();
            var opposingTeamId = await query.Select(o => o.HostTeamId).ToListAsync();
            var matchSettings = await this.db.MatchSettings.Where(o => hostTeamId.Contains(o.Id) || opposingTeamId.Contains(o.Id)).ToDictionaryAsync(k => k.Id);
            var count = await query.CountAsync();
            var matchs = await query.Skip(req.Skip).Take(req.Take).ToListAsync();
            return new ListMatchRes() {
                Count = count,
                Items = matchs.Select(o => MatchDto.FromEntity(o, matchSettings.GetValueOrDefault(o.HostTeamId), matchSettings.GetValueOrDefault(o.OppsingTeamId), this.current.UserId)).ToList(),
            };
        }

        public async Task<MatchDto?> Update(MatchDto model, EUpdateType type) {
            var isExited = await this.db.Matches.AnyAsync(o => o.Id == model.Id);
            ManagedException.ThrowIf(!isExited, Messages.Match.HostTeam_NotFount);
            var match = await this.db.Matches.Include(o => o.HostTeam).Include(o => o.OpposingTeam).FirstOrDefaultAsync(o => o.Id == model.Id);
            return MatchDto.FromEntity(match!);
        }
    }
}