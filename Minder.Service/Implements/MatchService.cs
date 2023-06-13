using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.Common;
using Minder.Service.Models.Match;
using Minder.Service.Models.Team;
using Minder.Services.Common;
using Minder.Services.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Minder.Service.Enums;

namespace Minder.Service.Implements {

    public class MatchService : BaseService, IMatchService {
        private readonly ITeamService teamService;
        private readonly AdministrativeUnitResource administrativeUnit;

        public MatchService(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.teamService = serviceProvider.GetRequiredService<ITeamService>();
            this.administrativeUnit = serviceProvider.GetRequiredService<AdministrativeUnitResource>();
        }

        public async Task<MatchDto?> Get(string matchId) {
            var isExited = await this.db.Matches.AnyAsync(O => O.Id == matchId);
            ManagedException.ThrowIf(!isExited, Messages.Match.Match_NotFound);
            var entity = await this.db.Matches.Include(o => o.HostTeam).Include(o => o.OpposingTeam)
                .FirstOrDefaultAsync(o => o.Id == matchId);

            var matchSettings = await this.db.MatchSettings.Include(o => o.Stadium).Include(o => o.Team).ThenInclude(o => o!.GameSetting).AsNoTracking()
                .Where(o => o.Id == entity!.OppsingTeamId || o.Id == entity.HostTeamId).ToDictionaryAsync(k => k.Id);

            // Get time option
            var timeChooices = (List<TimeChooice>?)await this.teamService.Automation(EAutoMation.Time, entity!.HostTeam!.TeamId, entity.OpposingTeam!.TeamId, true);
            var teamIds = matchSettings.Values.Select(o => o.TeamId).ToList();
            var myTeamId = await this.db.Members.Where(o => teamIds.Contains(o.TeamId) && o.UserId == this.current.UserId).Select(o => o.TeamId).FirstOrDefaultAsync();

            var timeOptions = timeChooices?.Select(o => MatchTimeOpption.FromTimeChooice(o)).Where(o => o.Opptions != null && o.Opptions.Any()).ToList();
            var match = MatchDto.FromEntity(entity, matchSettings.GetValueOrDefault(entity.HostTeamId), matchSettings.GetValueOrDefault(entity.OppsingTeamId), myTeamId, timeOptions, administrativeUnit);

            var itemIds = new List<string>();

            if (!string.IsNullOrEmpty(match.HostTeam?.TeamId)) itemIds.Add(match.HostTeam.TeamId);
            if (!string.IsNullOrEmpty(match.OpposingTeam?.TeamId)) itemIds.Add(match.OpposingTeam.TeamId);
            if (!string.IsNullOrEmpty(match.OpposingTeam?.StadiumId)) itemIds.Add(match.OpposingTeam.StadiumId);
            if (!string.IsNullOrEmpty(match.HostTeam?.StadiumId)) itemIds.Add(match.HostTeam.StadiumId);

            var files = await this.db.Files.Where(o => itemIds.Contains(o.ItemId) && (o.ItemType == EItemType.TeamAvatar || o.ItemType == EItemType.StadiumAvatar))
                .ToDictionaryAsync(k => k.ItemId!, v => $"{this.current.Url}/{v.Path}");

            if (!string.IsNullOrEmpty(match.HostTeam?.TeamId))
                match.HostTeam.Avatar = files.GetValueOrDefault(match.HostTeam.TeamId);
            if (!string.IsNullOrEmpty(match.OpposingTeam?.TeamId))
                match.OpposingTeam.Avatar = files.GetValueOrDefault(match.OpposingTeam.TeamId);

            if (!string.IsNullOrEmpty(match.HostTeam?.StadiumId))
                match.HostTeam.Stadium!.Avatar = files.GetValueOrDefault(match.HostTeam.StadiumId);
            if (!string.IsNullOrEmpty(match.OpposingTeam?.StadiumId))
                match.OpposingTeam.Stadium!.Avatar = files.GetValueOrDefault(match.OpposingTeam.StadiumId);

            return match;
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
            var hasPermistion = await this.db.Members.AsNoTracking()
                .AnyAsync(o => o.TeamId == req.HostTeamId && o.UserId == this.current.UserId
                            && (o.Regency == ERegency.Owner || o.Regency == ERegency.Captain));
            ManagedException.ThrowIf(!hasPermistion, Messages.Team.Team_NoPermistion);

            var teamIds = new List<string>() { req.HostTeamId, req.OpposingTeamId };
            var match = await this.db.Matches.FirstOrDefaultAsync(o => o.Status == EMatch.WaitingConfirm
                && teamIds.Contains(o.OpposingTeam!.TeamId) && teamIds.Contains(o.HostTeam!.TeamId));
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

            var query = this.db.Matches.AsNoTracking().Where(o => o.Status != EMatch.WaitingConfirm).Where(o => o.HostTeam!.TeamId == req.TeamId || o.OpposingTeam!.TeamId == req.TeamId);
            var hostTeamId = await query.Select(o => o.HostTeamId).ToListAsync();
            var opposingTeamId = await query.Select(o => o.OppsingTeamId).ToListAsync();
            var matchSettings = await this.db.MatchSettings.Include(o => o.Team).Where(o => hostTeamId.Contains(o.Id) || opposingTeamId.Contains(o.Id)).ToDictionaryAsync(k => k.Id);
            var count = await query.CountAsync();
            var matchs = await query.Skip(req.Skip).Take(req.Take).ToListAsync();

            var res = matchs.Select(o => MatchDto.FromEntity(o, matchSettings.GetValueOrDefault(o.HostTeamId), matchSettings.GetValueOrDefault(o.OppsingTeamId), req.TeamId)).ToList();

            var hostTeamIds = res.Where(o => !string.IsNullOrEmpty(o.HostTeam?.TeamId)).Select(o => o.HostTeam!.TeamId!).ToList();
            var oppositeIds = res.Where(o => !string.IsNullOrEmpty(o.OpposingTeam?.TeamId)).Select(o => o.OpposingTeam!.TeamId!).ToList();
            var teamIds = hostTeamIds.Union(oppositeIds).Distinct().ToList();

            var avatar = await this.db.Files.Where(o => teamIds.Contains(o.ItemId) && o.ItemType == EItemType.TeamAvatar)
                .ToDictionaryAsync(k => k.ItemId!, v => $"{this.current.Url}/{v.Path}");

            foreach (var item in res) {
                if (!string.IsNullOrEmpty(item.HostTeam?.TeamId))
                    item.HostTeam.Avatar = avatar.GetValueOrDefault(item.HostTeam.TeamId);
                if (!string.IsNullOrEmpty(item.OpposingTeam?.TeamId))
                    item.OpposingTeam.Avatar = avatar.GetValueOrDefault(item.OpposingTeam.TeamId);
            }

            return new ListMatchRes() {
                Count = count,
                Items = res,
            };
        }

        public async Task Void(string id) {
            var match = await this.db.Matches.FirstOrDefaultAsync(o => o.Id == id);

            ManagedException.ThrowIf(match == null, Messages.Match.Match_NotFount);

            match.Status = EMatch.Cancel;
            await this.db.SaveChangesAsync();
        }

        public async Task<object> Check(string matchId) {
            var entity = await this.db.Matches.Include(o => o.HostTeam).ThenInclude(o => o!.Stadium)
                .Include(o => o.OpposingTeam).ThenInclude(o => o!.Stadium)
                .FirstOrDefaultAsync(o => o.Id == matchId);
            ManagedException.ThrowIf(entity == null, Messages.Match.Match_NotFount);
            var hostTeam = entity.HostTeam;
            var opposingTeam = entity.OpposingTeam;

            if (hostTeam!.Date.Date != opposingTeam!.Date.Date || hostTeam.Date >= DateTimeOffset.UtcNow) {
                return new {
                    Status = false,
                    Type = "Time",
                    Message = "Vui lòng thống nhất thời gian"
                };
            }

            if (hostTeam.From != opposingTeam.From || hostTeam.To != opposingTeam.To) {
                return new {
                    Status = false,
                    Type = "Time",
                    Message = "Vui lòng thống nhất thời gian"
                };
            }

            if (hostTeam!.Stadium?.Id != opposingTeam!.Stadium?.Id || string.IsNullOrEmpty(hostTeam.Stadium?.Id)) {
                return new {
                    Status = false,
                    Type = "Stadium",
                    Message = "Vui lòng thống nhất sân đấu"
                };
            }
            entity.Status = EMatch.Complete;
            await this.db.SaveChangesAsync();

            return new {
                Status = true,
                Type = "None",
                Message = ""
            };
        }

        public async Task<MatchDto?> Update(string matchId, string teamId, object model, EUpdateType type) {
            var isExited = await this.db.Matches.AnyAsync(o => o.Id == matchId);
            ManagedException.ThrowIf(!isExited, Messages.Match.HostTeam_NotFount);

            var hasPermistion = await this.db.Members.AsNoTracking()
                .AnyAsync(o => o.TeamId == teamId && o.UserId == this.current.UserId
                            && (o.Regency == ERegency.Owner || o.Regency == ERegency.Captain));
            ManagedException.ThrowIf(!hasPermistion, Messages.Team.Team_NoPermistion);

            var entity = await this.db.Matches
               .Include(o => o.HostTeam).ThenInclude(o => o!.Stadium)
               .Include(o => o.HostTeam).ThenInclude(o => o!.MatchParticipants)
               .Include(o => o.OpposingTeam).ThenInclude(o => o!.Stadium)
               .Include(o => o.OpposingTeam).ThenInclude(o => o!.MatchParticipants)
               .FirstOrDefaultAsync(o => o.Id == matchId);

            switch (type) {
                case EUpdateType.Stadium:
                    var stadiumReq = (UpdateStadiumReq)model;
                    var isExitedStadium = await this.db.Stadiums.AnyAsync(o => o.Id == stadiumReq.StadiumId);
                    ManagedException.ThrowIf(!isExitedStadium, Messages.Stadium.Stadium_NotFound);
                    if (entity!.HostTeam!.TeamId == teamId) {
                        entity.HostTeam.StadiumId = stadiumReq.StadiumId;
                    } else if (entity!.OpposingTeam!.TeamId == teamId) {
                        entity.OpposingTeam.StadiumId = stadiumReq.StadiumId;
                    }
                    break;

                case EUpdateType.Time:
                    var timeReq = (UpdateTimeReq)model;
                    if (entity!.HostTeam!.TeamId == teamId) {
                        entity.HostTeam.SelectedDayOfWeek = timeReq.DayOfWeek;
                        entity.HostTeam.From = Enum.Parse<ETime>(timeReq.From.ToString());
                        entity.HostTeam.To = Enum.Parse<ETime>(timeReq.To.ToString());
                        entity.HostTeam.Date = timeReq.Date;
                    } else if (entity!.OpposingTeam!.TeamId == teamId) {
                        entity.OpposingTeam.SelectedDayOfWeek = timeReq.DayOfWeek;
                        entity.OpposingTeam.From = Enum.Parse<ETime>(timeReq.From.ToString());
                        entity.OpposingTeam.To = Enum.Parse<ETime>(timeReq.To.ToString());
                        entity.OpposingTeam.Date = timeReq.Date;
                    }
                    break;
            }
            await this.db.SaveChangesAsync();
            return await this.Get(matchId);
        }
    }
}