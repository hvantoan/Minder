using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.Group;
using Minder.Service.Models.Team;
using Minder.Services.Common;
using Minder.Services.Extensions;
using Minder.Services.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class TeamService : BaseService, ITeamService {
        private readonly IGroupService groupService;

        public TeamService(IServiceProvider serviceProvider) : base(serviceProvider) {
            groupService = serviceProvider.GetRequiredService<IGroupService>();
        }

        public async Task<ListTeamRes> List(ListTeamReq req) {
            var members = this.db.Members.AsNoTracking().WhereIf(req.IsMyTeam, o => o.UserId == this.current.UserId)
                .WhereIf(req.teamIds != null, o => req.teamIds!.Contains(o.TeamId));
            var teamIds = members.Select(o => o.TeamId);
            var query = this.db.Teams.Include(o => o.GameSetting).AsNoTracking().Where(o => teamIds.Contains(o.Id));

            if (!string.IsNullOrEmpty(req.SearchText)) {
                req.SearchText = req.SearchText.ReplaceSpace(isUnsignedUnicode: true);
                query = query.Where(o => o.Name.Contains(req.SearchText) || o.Name.GetSumary().Contains(req.SearchText) || o.Code.ToLower().Contains(req.SearchText));
            }

            var items = await query.OrderBy(o => o.Id).Skip(req.PageIndex * req.PageSize).Take(req.PageSize).Select(o => TeamDto.FromEntity(o)).ToListAsync();
            var data = await members.Where(o => o.UserId == this.current.UserId).ToListAsync();
            foreach (var item in items) {
                if (item != null) {
                    item.GameSetting = null;
                    item.Regency = data.FirstOrDefault(o => o.TeamId == item.Id)?.Regency;
                }
            }

            return new ListTeamRes() {
                Count = await query.CountIf(req.IsCount, o => o.Id),
                Items = items
            };
        }

        public async Task<TeamDto?> Get(string teamId) {
            var team = await this.db.Teams.Include(o => o.GameSetting).ThenInclude(o => o!.GameTime).Include(o => o.Members).FirstOrDefaultAsync(o => o.Id == teamId);
            ManagedException.ThrowIf(team == null, Messages.Team.Team_NotFound);
            return TeamDto.FromEntity(team);
        }

        public async Task<string> CreateOrUpdate(TeamDto model) {
            if (string.IsNullOrEmpty(model.Id)) {
                return await this.Create(model);
            } else {
                return await this.Update(model);
            }
        }

        private async Task<string> Create(TeamDto model) {
            this.logger.Information($"{nameof(Team)} - {nameof(Create)} - Start", model);

            var isOwner = await this.db.Members.AnyAsync(o => o.UserId == this.current.UserId && o.Regency == ERegency.Owner);
            ManagedException.ThrowIf(isOwner, Messages.Team.Team_IsOwner);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Code), Messages.Team.Team_CodeRequired);
            ManagedException.ThrowIf(model.Code.Length > 32 || model.Code.Length < 2, Messages.Team.Team_CodeRequired);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Code), Messages.Team.Team_CodeRequired);
            ManagedException.ThrowIf(model.Code.Length > 4 || model.Code.Length < 2, Messages.Team.Team_CodeRequired);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.Team.Team_NameRequired);
            ManagedException.ThrowIf(model.Code.Length > 80, Messages.Team.Team_DescriptionRequired);

            var isExitCode = await this.db.Teams.Include(o => o.GameSetting).ThenInclude(o => o!.GameTime).AnyAsync(o => o.Code == model.Code);
            ManagedException.ThrowIf(isExitCode, Messages.Team.Team_CodeExited);

            model.GameSetting ??= new();
            if (!model.GameSetting.GameTypes.Any()) model.GameSetting.GameTypes.Add(EGameType.Five);

            var team = new Team() {
                Id = Guid.NewGuid().ToStringN(),
                Code = model.Code,
                Name = model.Name,
                GameSetting = new GameSetting() {
                    Id = Guid.NewGuid().ToStringN(),
                    GameTypes = JsonConvert.SerializeObject(model.GameSetting.GameTypes),
                    Positions = JsonConvert.SerializeObject(model.GameSetting.Positions),
                    GameTime = model.GameSetting.GameTime?.ToEntity() ?? new() { Id = Guid.NewGuid().ToStringN() },
                    Longitude = model.GameSetting.Longitude,
                    Latitude = model.GameSetting.Latitude,
                    Radius = model.GameSetting.Radius,
                    Rank = model.GameSetting.Rank,
                    Point = model.GameSetting.Point,
                },
                CreateAt = DateTimeOffset.Now,
            };

            var member = new Member() {
                Id = Guid.NewGuid().ToStringN(),
                Regency = ERegency.Owner,
                TeamId = team.Id,
                UserId = this.current.UserId
            };

            await this.db.Teams.AddAsync(team);
            await this.db.Members.AddAsync(member);
            await this.db.SaveChangesAsync();

            await this.groupService.Create(new GroupDto() {
                UserIds = team.Members?.Select(o => o.UserId).ToList(),
                TeamId = team.Id,
                Title = team.Name,
            });

            this.logger.Information($"{nameof(Team)} - {nameof(Create)} - End", team.Id);

            return team.Id;
        }

        public async Task<string> Update(TeamDto model) {
            this.logger.Information($"{nameof(Team)} - {nameof(Update)} - Start", model);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Code), Messages.Team.Team_CodeRequired);
            ManagedException.ThrowIf(model.Code.Length > 32 || model.Code.Length < 2, Messages.Team.Team_CodeRequired);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Code), Messages.Team.Team_CodeRequired);
            ManagedException.ThrowIf(model.Code.Length > 4 || model.Code.Length < 2, Messages.Team.Team_CodeRequired);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.Team.Team_NameRequired);
            ManagedException.ThrowIf(model.Code.Length > 80, Messages.Team.Team_DescriptionRequired);

            var isExitCode = await this.db.Teams.AnyAsync(o => o.Code == model.Code && o.Id != model.Id);
            ManagedException.ThrowIf(isExitCode, Messages.Team.Team_CodeExited);

            var team = await this.db.Teams.Include(o => o.GameSetting).ThenInclude(o => o!.GameTime).Include(o => o.Members).FirstOrDefaultAsync(o => o.Id == model.Id);
            ManagedException.ThrowIf(team == null, Messages.Team.Team_NotFound);
            var myRegency = team.Members?.FirstOrDefault(o => o.UserId == this.current.UserId);
            ManagedException.ThrowIf(myRegency == null || myRegency.Regency == ERegency.Member, Messages.Team.Team_NoPermistion);

            if (!string.IsNullOrWhiteSpace(model.Code)) team.Code = model.Code;
            if (!string.IsNullOrWhiteSpace(model.Name)) team.Name = model.Name;
            if (model.GameSetting != null) {
                if (model.GameSetting.GameTime != null) team.GameSetting!.GameTime = model.GameSetting.GameTime.ToEntity();
                if (model.GameSetting.GameTypes != null) team.GameSetting!.GameTypes = JsonConvert.SerializeObject(model.GameSetting.GameTypes);
                if (model.GameSetting.Positions != null) team.GameSetting!.Positions = JsonConvert.SerializeObject(model.GameSetting.Positions);
                if (model.GameSetting.Longitude != decimal.Zero) team.GameSetting!.Longitude = model.GameSetting.Longitude;
                if (model.GameSetting.Latitude != decimal.Zero) team.GameSetting!.Latitude = model.GameSetting.Latitude;
                if (model.GameSetting.Radius != 0.0) team.GameSetting!.Radius = model.GameSetting.Radius;
                if (model.GameSetting.Rank != ERank.None) team.GameSetting!.Rank = model.GameSetting.Rank;
            }

            await this.db.SaveChangesAsync();
            this.logger.Information($"{nameof(Team)} - {nameof(Create)} - End", team.Id);

            return team.Id;
        }

        public async Task Delete(string teamId) {
            this.logger.Information($"{nameof(Team)} - {nameof(Delete)} - Start", teamId);

            var team = await this.db.Teams.Include(o => o.Members).Include(o => o.GameSetting).Include(o => o.Groups).FirstOrDefaultAsync(o => o.Id == teamId);
            ManagedException.ThrowIf(team == null, Messages.Team.Team_NotFound);

            var isOwner = await this.db.Members.AnyAsync(o => o.UserId == this.current.UserId && o.Regency == ERegency.Owner && o.TeamId == teamId);
            ManagedException.ThrowIf(!isOwner, Messages.Team.Team_NoPermistion);

            this.db.Teams.Remove(team!);
            await this.db.SaveChangesAsync();
        }

        public async Task Leave(string teamId) {
            var isInTeam = await this.db.Members.AnyAsync(o => o.TeamId == teamId && o.UserId == this.current.UserId);
            ManagedException.ThrowIf(!isInTeam, Messages.System.System_Error);

            var isOwner = await this.db.Members.AnyAsync(o => o.TeamId == teamId && o.UserId == this.current.UserId && o.Regency == ERegency.Owner);
            ManagedException.ThrowIf(isOwner, Messages.Team.Team_IsOwner);

            var team = await this.db.Teams.Include(o => o.Members).Include(o => o.Groups!).ThenInclude(o => o.Participants).FirstOrDefaultAsync(o => o.Id == teamId);
            if (team != null) {
                team.Members = team.Members!.Where(o => o.UserId != this.current.UserId).ToList();
                var group = team.Groups!.FirstOrDefault(o => string.IsNullOrEmpty(o.ChannelId));
                group!.Participants = group!.Participants!.Where(o => o.UserId != this.current.UserId).ToList();
                await this.db.SaveChangesAsync();
            }
        }

        public async Task Kick(string userId) {
            var member = await this.db.Members.AsNoTracking().FirstOrDefaultAsync(o => o.UserId == this.current.UserId && o.Regency == ERegency.Owner);
            ManagedException.ThrowIf(string.IsNullOrEmpty(member?.Id), Messages.Team.Team_NotFound);
            ManagedException.ThrowIf(userId == this.current.UserId, Messages.System.System_Error);

            var memberKick = await this.db.Members.FirstOrDefaultAsync(o => o.TeamId == member.TeamId && o.UserId == userId);
            ManagedException.ThrowIf(memberKick == null, Messages.Team.Team_NotInTeam);

            var team = await this.db.Teams.Include(o => o.Members).Include(o => o.Groups!).ThenInclude(o => o.Participants).FirstOrDefaultAsync(o => o.Id == member.TeamId);
            if (team != null) {
                team.Members = team.Members!.Where(o => o.UserId != userId).ToList();
                var group = team.Groups!.FirstOrDefault(o => string.IsNullOrEmpty(o.ChannelId));
                group!.Participants = group!.Participants!.Where(o => o.UserId != userId).ToList();
                await this.db.SaveChangesAsync();
            }
            await this.db.SaveChangesAsync();
        }

        public async Task<ListTeamRes> Find(FindTeamReq req) {
            List<string> hour = new();
            var query = this.db.Teams.Include(o => o.GameSetting).ThenInclude(o => o!.GameTime).Include(o => o.Members)
                    .WhereIf(req.Rank.HasValue, o => o.GameSetting!.Rank == req.Rank)
                    .WhereIf(req.Position.HasValue, o => o.GameSetting!.Positions.Contains(req.Position!.ToString() ?? ""))
                    .WhereIf(req.GameType.HasValue, o => o.GameSetting!.GameTypes.Contains(req.GameType!.ToString() ?? ""))
                    .WhereIf(req.Member.HasValue, o => o.Members!.Count >= req.Member);

            var teams = await query.ToListAsync();
            var members = teams.SelectMany(o => o.Members!);
            var userIds = members.Where(o => !string.IsNullOrEmpty(o.UserId)).Select(o => o.UserId).Distinct().ToList();

            var users = await this.db.Users.Where(o => userIds.Contains(o.Id)).ToDictionaryAsync(k => k.Id, v => { return GetAge(v.DayOfBirth); });
            var teamIds = new List<string>();

            if (req.Age.HasValue || (req.Day.HasValue && req.Time.HasValue)) {
                foreach (var team in teams) {
                    //Filter Age
                    var isAgeAdd = false;
                    if (req.Age.HasValue) {
                        var ageTotal = 0;
                        foreach (var member in team.Members!) {
                            ageTotal += users.GetValueOrDefault(member.UserId);
                        }

                        var ageAve = ageTotal / members.Count();
                        switch (req.Age) {
                            case EAge.Children:
                                isAgeAdd = ageAve < 16;
                                break;

                            case EAge.Youth:
                                isAgeAdd = 16 <= ageAve && ageAve <= 25;
                                break;

                            case EAge.Middle:
                                isAgeAdd = 25 <= ageAve && ageAve <= 35;
                                break;

                            case EAge.Old:
                                isAgeAdd = ageAve > 35;
                                break;
                        }
                    }

                    //Fillter Time
                    var isTimeAdd = true;

                    if (req.Day.HasValue && req.Time.HasValue) {
                        var item = TeamDto.FromEntity(team);
                        var (from, to) = GetTimeFromOption(req.Time.Value);

                        switch (req.Day) {
                            case EDay.Monday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Monday.Any(o => from <= o && o <= to);
                                break;

                            case EDay.Tuesday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Tuesday.Any(o => from <= o && o <= to);
                                break;

                            case EDay.Wednesday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Wednesday.Any(o => from <= o && o <= to);
                                break;

                            case EDay.Thursday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Thursday.Any(o => from <= o && o <= to);
                                break;

                            case EDay.Friday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Friday.Any(o => from <= o && o <= to);
                                break;

                            case EDay.Saturday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Saturday.Any(o => from <= o && o <= to);
                                break;

                            case EDay.Sunday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Sunday.Any(o => from <= o && o <= to);
                                break;

                            case null:
                                isTimeAdd = false;
                                break;
                        }
                    }

                    if (isAgeAdd && isTimeAdd) {
                        teamIds.Add(team.Id);
                    }
                }
            } else {
                teamIds = teams.Select(o => o.Id).ToList();
            }

            return await List(new ListTeamReq() {
                IsCount = true,
                IsMyTeam = false,
                PageIndex = req.PageIndex,
                PageSize = req.PageSize,
                teamIds = teamIds
            });
        }

        public (EGameTime, EGameTime) GetTimeFromOption(ETime time) {
            var from = EGameTime.One;
            var to = EGameTime.One;

            switch (time) {
                case ETime.OptionOne:
                    from = EGameTime.One;
                    to = EGameTime.Six;
                    break;

                case ETime.OptionTwo:
                    from = EGameTime.Seven;
                    to = EGameTime.Twelve;
                    break;

                case ETime.OptionThree:
                    from = EGameTime.Thirteen;
                    to = EGameTime.Eighteen;
                    break;

                case ETime.OptionFour:
                    from = EGameTime.Nine;
                    to = EGameTime.TwentyFour;
                    break;
            }

            return (from, to);
        }

        public int GetAge(DateTimeOffset dateOfBirth) {
            DateTimeOffset today = DateTimeOffset.Now;
            int age = today.Year - dateOfBirth.Year;

            if (dateOfBirth > today.AddYears(-age)) {
                age--;
            }

            return age;
        }
    }
}