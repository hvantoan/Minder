using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.Common;
using Minder.Service.Models.Match;
using Minder.Service.Models.Stadium;
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
        private readonly IStadiumService stadiumService;
        private readonly AdministrativeUnitResource administrativeUnit;

        public MatchService(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.teamService = serviceProvider.GetRequiredService<ITeamService>();
            this.stadiumService = serviceProvider.GetRequiredService<IStadiumService>();
            this.administrativeUnit = serviceProvider.GetRequiredService<AdministrativeUnitResource>();
        }

        public async Task MemberConfirm(string userId, string matchId) {
            var match = await this.db.Matches.Include(o => o.Participants).FirstOrDefaultAsync(o => o.Id == matchId && o.Status == EMatch.WaitingMemberConfirm);
            if (match != null) {
                match.Participants ??= new List<MatchParticipant>();
                var isExit = match.Participants.Any(o => o.UserId == userId);
                if (isExit) {
                    var participant = match.Participants.FirstOrDefault(o => o.UserId == userId);
                    participant!.IsAccept = !participant.IsAccept;
                } else {
                    match.Participants.Add(new MatchParticipant() {
                        Id = Guid.NewGuid().ToStringN(),
                        IsAccept = true,
                        MatchId = matchId,
                        Position = 0,
                        UserId = userId,
                    });
                }
                await this.db.SaveChangesAsync();
            }
        }

        public async Task AddTimeOpption(AddTimeOpptionReq req) {
            var match = await this.db.Matches.AsNoTracking().FirstOrDefaultAsync(o => o.Id == req.MatchId && o.Status == EMatch.Prepare);
            if (match != null) {
                var item = new TimeOpption() {
                    Id = Guid.NewGuid().ToStringN(),
                    Date = req.Date,
                    DayOfWeek = (EDayOfWeek)req.Date.DayOfWeek,
                    MatchId = req.MatchId,
                    TimeItems = new List<TimeItem>() {
                        new TimeItem() {
                            Id = Guid.NewGuid().ToStringN(),
                            From = req.From,
                            To = req.To,
                            MemberCount = 0
                        }
                    }
                };
                await this.db.TimeOpptions.AddAsync(item);
                await this.db.SaveChangesAsync();
            }
        }

        public async Task ConfirmSettingMatch(string matchId, string teamId) {
            var match = await this.db.Matches.Include(o => o.HostTeam).Include(o => o.OpposingTeam).Include(o => o.Participants).FirstOrDefaultAsync(o => o.Id == matchId);
            if (match != null) {
                if (match.HostTeam != null && match.HostTeam.TeamId == teamId && match.HostTeam.From != null
                    && match.HostTeam.To != null && match.HostTeam.StadiumId != null) {
                    match.HostTeam.HasConfirm = !match.HostTeam.HasConfirm;
                }

                if (match.OpposingTeam != null && match.OpposingTeam.TeamId == teamId && match.OpposingTeam.From != null
                    && match.OpposingTeam.To != null && match.OpposingTeam.StadiumId != null) {
                    match.OpposingTeam.HasConfirm = !match.OpposingTeam.HasConfirm;
                }

                if (match.OpposingTeam!.HasConfirm && match.OpposingTeam.HasConfirm) {
                    match.StadiumId = match.HostTeam!.StadiumId;
                    match.SelectedDayOfWeek = match.HostTeam!.SelectedDayOfWeek;
                    match.From = match.HostTeam!.From;
                    match.To = match.HostTeam!.To;
                    match.SelectedDate = match!.HostTeam.Date;
                    match.Status = EMatch.WaitingMemberConfirm;
                } else {
                    match.StadiumId = null;
                    match.SelectedDayOfWeek = null;
                    match.From = null;
                    match.To = null;
                    match.SelectedDate = DateTimeOffset.MinValue;
                    match.Participants = null;
                    match.Status = EMatch.Prepare;
                }
            }
            await this.db.SaveChangesAsync();
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

            var timeOptions = timeChooices?.Select(o => TimeOpptionDto.FromTimeChooice(o)).Where(o => o.Opptions != null && o.Opptions.Any()).ToList();
            var timeCustom = await this.db.TimeOpptions.Include(o => o.TimeItems).AsNoTracking().Where(o => o.MatchId == entity.Id).ToListAsync();

            var timeOpptionRes = new List<TimeOpptionDto>();
            if (timeOptions != null) {
                foreach (var time in timeCustom) {
                    if (time.TimeItems == null || !time.TimeItems.Any()) continue;
                    var hasDay = timeOptions.FirstOrDefault(o => o.Date.Date == time.Date.Date);
                    if (hasDay != null) {
                        var temp = time.TimeItems!.Select(o => new TimeItemDto() {
                            From = o.From,
                            To = o.To,
                            MemberCount = 0
                        });

                        hasDay.Opptions ??= new();
                        hasDay.Opptions.AddRange(temp);
                        timeOpptionRes.Add(hasDay);
                    } else {
                        var timeOpp = new TimeOpptionDto() {
                            Date = time.Date,
                            DayOfWeek = time.DayOfWeek,
                            Opptions = time.TimeItems!.Select(o => new TimeItemDto() {
                                From = o.From,
                                To = o.To,
                                MemberCount = 0
                            }).ToList()
                        };
                    }
                }
            }
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

            var matchIds = matchs.Select(o => o.Id).ToList();
            var matchParticipants = await this.db.MatchParticipants.AsNoTracking().Where(o => matchIds.Contains(o.MatchId)).ToListAsync();
            var userIds = matchParticipants.Select(o => o.UserId).OrderBy(o => o).Distinct().ToList();

            var images = await this.db.Files.Where(o => userIds.Contains(o.ItemId) && o.ItemType == EItemType.UserAvatar && o.Type == EFile.Image)
                .ToDictionaryAsync(k => k.ItemId, v => $"{this.current.Url}/{v.Path}");

            var participants = matchParticipants.Select(o => new MatchParticipantDto() {
                Avatar = images.GetValueOrDefault(o.UserId),
                Email = o.User?.Username,
                Phone = o.User?.Phone,
                Name = o.User?.Name,
                UserId = o.UserId,
                MatchId = o.MatchId
            }).GroupBy(o => o.MatchId).ToDictionary(k => k.Key, v => v.ToList());

            var res = matchs.Select(o => MatchDto.FromEntity(o, matchSettings.GetValueOrDefault(o.HostTeamId), matchSettings.GetValueOrDefault(o.OppsingTeamId), req.TeamId, participants: participants)).ToList();
            var hostTeamIds = res.Where(o => !string.IsNullOrEmpty(o.HostTeam?.TeamId)).Select(o => o.HostTeam!.TeamId!).ToList();
            var oppositeIds = res.Where(o => !string.IsNullOrEmpty(o.OpposingTeam?.TeamId)).Select(o => o.OpposingTeam!.TeamId!).ToList();
            var teamIds = hostTeamIds.Union(oppositeIds).Distinct().ToList();

            var avatar = await this.db.Files.Where(o => teamIds.Contains(o.ItemId) && o.ItemType == EItemType.TeamAvatar)
                .ToDictionaryAsync(k => k.ItemId!, v => $"{this.current.Url}/{v.Path}");

            var stadiumIds = res.Where(o => o.SelectedStadiumId != null).Select(o => o.SelectedStadiumId!).ToList();

            var stadium = new List<StadiumDto>();
            if (stadiumIds != null && stadiumIds.Any()) {
                var data = await this.stadiumService.List(new Models.Stadium.ListStadiumReq() {
                    StadiumIds = stadiumIds,
                });
                stadium = data.Items;
            }

            foreach (var item in res) {
                if (!string.IsNullOrEmpty(item.HostTeam?.TeamId))
                    item.HostTeam.Avatar = avatar.GetValueOrDefault(item.HostTeam.TeamId);
                if (!string.IsNullOrEmpty(item.OpposingTeam?.TeamId))
                    item.OpposingTeam.Avatar = avatar.GetValueOrDefault(item.OpposingTeam.TeamId);

                if (!string.IsNullOrEmpty(item.SelectedStadiumId))
                    item.Stadium = stadium.FirstOrDefault(o => o.Id == item.SelectedStadiumId);
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

        public async Task<MatchDto?> Update(string matchId, string teamId, object model, EUpdateType type) {
            var isExited = await this.db.Matches.AnyAsync(o => o.Id == matchId);
            ManagedException.ThrowIf(!isExited, Messages.Match.HostTeam_NotFount);

            var hasPermistion = await this.db.Members.AsNoTracking()
                .AnyAsync(o => o.TeamId == teamId && o.UserId == this.current.UserId
                            && (o.Regency == ERegency.Owner || o.Regency == ERegency.Captain));
            ManagedException.ThrowIf(!hasPermistion, Messages.Team.Team_NoPermistion);

            var entity = await this.db.Matches.Include(o => o.HostTeam).ThenInclude(o => o!.Stadium)
                           .Include(o => o.OpposingTeam).ThenInclude(o => o!.Stadium)
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