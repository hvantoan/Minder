using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.Common;
using Minder.Service.Models.File;
using Minder.Service.Models.GameSetting;
using Minder.Service.Models.Group;
using Minder.Service.Models.Team;
using Minder.Service.Models.User;
using Minder.Services.Common;
using Minder.Services.Extensions;
using Minder.Services.Interfaces;
using Minder.Services.Models.User;
using Minder.Services.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Minder.Service.Enums;

namespace Minder.Service.Implements {

    public class TeamService : BaseService, ITeamService {
        private readonly IGroupService groupService;
        private readonly IFileService fileService;
        private readonly IUserService userService;

        public TeamService(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.groupService = serviceProvider.GetRequiredService<IGroupService>();
            this.fileService = serviceProvider.GetRequiredService<IFileService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
        }

        public async Task<ListUserRes> SuggestUserForTeam(ListUserSuggest req) {
            var isExited = await this.db.Teams.AnyAsync(o => o.Id == req.TeamId);
            ManagedException.ThrowIf(!isExited, Messages.Team.Team_NotFound);
            var entity = await this.db.Teams.Include(o => o.GameSetting).AsNoTracking().FirstOrDefaultAsync(o => o.Id == req.TeamId);
            var team = TeamDto.FromEntity(entity!);
            var memberIds = await this.db.Members.AsNoTracking().Where(o => o.TeamId == req.TeamId).Select(o => o.UserId).ToListAsync();
            var userHasRejects = await this.db.TeamRejecteds.Where(o => o.Type == ETeamRejected.User).Select(o => o.ItemId).ToListAsync();
            var userInviteds = await this.db.Invites.Where(o => o.TeamId == req.TeamId && o.Type == EInvitationType.Invite).Select(o => o.UserId).ToListAsync();

            var users = await this.db.Users.Include(o => o.GameSetting).Where(o => !memberIds.Contains(o.Id) && !userHasRejects.Contains(o.Id) && !userInviteds.Contains(o.Id))
                .Skip(req.Skip).Take(req.Take)
                .ToListAsync();

            var userIds = users.Select(o => o.Id).ToList();
            var file = await this.db.Files.Where(o => o.ItemType == EItemType.UserAvatar || o.ItemType == EItemType.UserCover && userIds.Contains(o.ItemId)).ToListAsync();
            var avatars = file.Where(o => o.ItemType == EItemType.UserAvatar).ToDictionary(k => k.ItemId, v => $"{this.current.Url}/{v.Path}");
            var covers = file.Where(o => o.ItemType == EItemType.UserCover).ToDictionary(k => k.ItemId, v => $"{this.current.Url}/{v.Path}");

            var responseUser = users.Select(o => UserDto.FromEntity(o, null, avatars.GetValueOrDefault(o.Id), covers.GetValueOrDefault(o.Id))).ToList();
            var res = MinderExtension.SortUserDistance(team!, responseUser!);
            return await this.userService.List(new ListUserReq() {
                PageIndex = 0,
                PageSize = res.Count,
                UserIds = res.Select(o => o.Id!).ToList(),
            });
        }

        public async Task<ListTeamRes> List(ListTeamReq req) {
            var members = this.db.Members.Include(o => o.User).AsNoTracking().WhereIf(req.IsMyTeam, o => o.UserId == this.current.UserId)
                .WhereIf(req.TeamIds != null, o => req.TeamIds!.Contains(o.TeamId));
            var teamIds = members.Select(o => o.TeamId);
            var query = this.db.Teams.AsNoTracking().Where(o => teamIds.Contains(o.Id));

            if (!string.IsNullOrEmpty(req.SearchText)) {
                req.SearchText = req.SearchText.ReplaceSpace(isUnsignedUnicode: true);
                query = query.Where(o => o.Name.Contains(req.SearchText) || o.Name.GetSumary().Contains(req.SearchText) || o.Code.ToLower().Contains(req.SearchText));
            }
            var items = await query.OrderBy(o => o.Id).Skip(req.PageIndex * req.PageSize).Take(req.PageSize).ToListAsync();
            var fileIds = items.Select(o => o.Id).ToList();
            var file = await this.db.Files.Where(o => o.Type == EFile.Image && (o.ItemType == EItemType.TeamAvatar || o.ItemType == EItemType.TeamCover) && fileIds.Contains(o.ItemId))
               .Select(o => FileDto.FromEntity(o, this.current.Url)).ToListAsync();
            var avatarFiles = file.Where(o => o!.ItemType == EItemType.TeamAvatar).GroupBy(o => o!.ItemId).ToDictionary(k => k.Key, v => v.FirstOrDefault()?.Path);
            var coverFiles = file.Where(o => o!.ItemType == EItemType.TeamCover).GroupBy(o => o!.ItemId).ToDictionary(k => k.Key, v => v.FirstOrDefault()?.Path);

            var response = items.Select(o => TeamDto.FromEntity(o, avatarFiles?.GetValueOrDefault(o.Id), coverFiles?.GetValueOrDefault(o.Id))).ToList();

            var data = await members.Where(o => o.UserId == this.current.UserId || o.Regency == ERegency.Owner).ToListAsync();
            foreach (var item in response) {
                if (item != null) {
                    item.Owner = data.FirstOrDefault(o => o.TeamId == item.Id && o.Regency == ERegency.Owner)?.User?.Name;
                    item.Regency = data.FirstOrDefault(o => o.TeamId == item.Id && o.UserId == this.current.UserId)?.Regency;
                }
            }

            return new ListTeamRes() {
                Count = await query.CountIf(req.IsCount, o => o.Id),
                Items = response!
            };
        }

        public async Task<object?> Automation(EAutoMation type, string hostTeamId, string? opposingTeamId = null, bool getTimeChooiceModel = false) {
            var userIds = await this.db.Members.Where(o => o.TeamId == hostTeamId || (!string.IsNullOrEmpty(opposingTeamId) && o.TeamId == opposingTeamId))
                .Select(o => o.UserId).ToListAsync();
            var users = await this.db.Users.Include(o => o.GameSetting).ThenInclude(o => o!.GameTime)
                .Where(o => userIds.Contains(o.Id))
                .Select(o => UserDto.FromEntity(o, null, null, null))
                .ToListAsync();

            switch (type) {
                case EAutoMation.Location:
                    return CalculatorCenterPoint(users!);

                case EAutoMation.Time:
                    return GetTime(users!, getTimeChooiceModel);
            }
            ManagedException.Throw(Messages.System.System_Error);
            return null;
        }

        public async Task<TeamDto?> Get(string teamId) {
            var team = await this.db.Teams.Include(o => o.GameSetting).ThenInclude(o => o!.GameTime).Include(o => o.Members!).ThenInclude(o => o.User).AsNoTracking().FirstOrDefaultAsync(o => o.Id == teamId);
            ManagedException.ThrowIf(team == null, Messages.Team.Team_NotFound);

            var group = await this.db.Groups.FirstOrDefaultAsync(o => o.ChannelId == team.Id && o.Type == EGroup.Team);
            var avatar = await this.fileService.Get(team.Id, EItemType.TeamAvatar);
            var coverAvatar = await this.fileService.Get(team.Id, EItemType.TeamCover);
            var userIds = team.Members?.Select(o => o.UserId).ToList();

            var userDtos = await this.userService.List(new ListUserReq() {
                PageIndex = 0,
                PageSize = userIds!.Count,
                UserIds = userIds
            });

            var res = TeamDto.FromEntity(team, avatar?.Path, coverAvatar?.Path, group?.Id);

            foreach (var item in res.Members ?? new List<MemberDto>()) {
                item.User = userDtos.Items.FirstOrDefault(o => o!.Id == item.UserId);
            }

            return res;
        }

        public async Task<TeamDto?> CreateOrUpdate(SaveTeamRequest model) {
            if (string.IsNullOrEmpty(model.Id)) {
                return await this.Create(model);
            } else {
                return await this.Update(model);
            }
        }

        private async Task<TeamDto?> Create(SaveTeamRequest model) {
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
                ChannelId = team.Id,
                Title = team.Name,
                Type = EGroup.Team
            });

            if (model.AvatarData != null && model.AvatarData.Any()) {
                await this.fileService.Create(new FileDto() {
                    Data = model.AvatarData,
                    ItemId = team.Id,
                    ItemType = EItemType.TeamAvatar,
                });
            }

            if (model.CoverData != null && model.CoverData.Any()) {
                await this.fileService.Create(new FileDto() {
                    Data = model.CoverData,
                    ItemId = team.Id,
                    ItemType = EItemType.TeamCover,
                });
            }

            this.logger.Information($"{nameof(Team)} - {nameof(Create)} - End", team.Id);

            return await Get(team.Id);
        }

        public async Task<TeamDto?> Update(TeamDto model) {
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
            team.IsAutoTime = model.IsAutoTime;
            team.IsAutoLocation = model.IsAutoLocation;

            if (model.IsAutoTime) {
                var gameTime = (GameTimeDto?)await Automation(EAutoMation.Time, team.Id);
                team.GameSetting!.GameTime = gameTime?.ToEntity() ?? new GameTime();
            }

            if (model.IsAutoLocation) {
                var point = (Point?)await Automation(EAutoMation.Location, team.Id);
                if (point != null) {
                    team.GameSetting!.Longitude = point.Longitude;
                    team.GameSetting!.Latitude = point.Latitude;
                }
            }

            await this.db.SaveChangesAsync();
            this.logger.Information($"{nameof(Team)} - {nameof(Create)} - End", team.Id);

            return await Get(team.Id);
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

            var memberLeave = await this.db.Members.FirstOrDefaultAsync(o => o.TeamId == teamId && o.UserId == this.current.UserId);
            memberLeave!.IsDeleted = true;

            var groupLeave = await this.db.Groups.Include(o => o.Participants).FirstOrDefaultAsync(o => o.Type == EGroup.Team && o.ChannelId == teamId);
            if (groupLeave != null) {
                var participant = groupLeave.Participants!.FirstOrDefault(o => o.UserId == this.current.UserId && o.GroupId == groupLeave.Id);
                if (participant != null) participant.IsDeleted = true;
            }
            await this.db.SaveChangesAsync();
        }

        public async Task Kick(string userId) {
            var member = await this.db.Members.AsNoTracking().FirstOrDefaultAsync(o => o.UserId == this.current.UserId && o.Regency == ERegency.Owner);
            ManagedException.ThrowIf(string.IsNullOrEmpty(member?.Id), Messages.Team.Team_NotFound);
            ManagedException.ThrowIf(userId == this.current.UserId, Messages.System.System_Error);

            var memberKick = await this.db.Members.FirstOrDefaultAsync(o => o.TeamId == member.TeamId && o.UserId == userId);
            ManagedException.ThrowIf(memberKick == null, Messages.Team.Team_NotInTeam);
            memberKick.IsDeleted = true;

            var groupLeave = await this.db.Groups.Include(o => o.Participants).FirstOrDefaultAsync(o => o.Type == EGroup.Team && o.ChannelId == memberKick.TeamId);
            if (groupLeave != null) {
                var participant = groupLeave.Participants!.FirstOrDefault(o => o.UserId == this.current.UserId);
                participant!.IsDeleted = true;
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

            if (req.Age.HasValue || (req.Day.HasValue && req.From > req.To)) {
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

                    if (req.Day.HasValue && req.From > req.To) {
                        var item = TeamDto.FromEntity(team);

                        switch (req.Day) {
                            case EDayOfWeek.Monday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Monday.Any(o => req.From <= o && o <= req.To);
                                break;

                            case EDayOfWeek.Tuesday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Tuesday.Any(o => req.From <= o && o <= req.To);
                                break;

                            case EDayOfWeek.Wednesday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Wednesday.Any(o => req.From <= o && o <= req.To);
                                break;

                            case EDayOfWeek.Thursday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Thursday.Any(o => req.From <= o && o <= req.To);
                                break;

                            case EDayOfWeek.Friday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Friday.Any(o => req.From <= o && o <= req.To);
                                break;

                            case EDayOfWeek.Saturday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Saturday.Any(o => req.From <= o && o <= req.To);
                                break;

                            case EDayOfWeek.Sunday:
                                isTimeAdd = item!.GameSetting!.GameTime!.Sunday.Any(o => req.From <= o && o <= req.To);
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
                TeamIds = teamIds
            });
        }

        public async Task<ListTeamRes> Suggestion(SuggessTeamReq req) {
            var entity = await this.db.Teams.Include(o => o.TeamRejecteds).Include(o => o.Members).Include(o => o.GameSetting).ThenInclude(o => o!.GameTime)
                .AsNoTracking().FirstOrDefaultAsync(o => o!.Id == req.TeamId);
            var myTeam = TeamDto.FromEntity(entity!);

            var queryTeams = this.db.Teams.Include(o => o.Members).Include(o => o.GameSetting).ThenInclude(o => o!.GameTime).AsNoTracking()
                .Where(o => o.Id != req.TeamId && !myTeam!.TeamRejectedId.Contains(o.Id));

            var teamIds = await queryTeams.Select(o => o.Id).ToListAsync();
            var files = await this.db.Files.Where(o => teamIds.Contains(o.ItemId) && (o.ItemType == EItemType.TeamAvatar || o.ItemType == EItemType.TeamCover)).ToListAsync();
            var avatars = files.Where(o => o.ItemType == EItemType.TeamAvatar).ToDictionary(k => k.ItemId, v => $"{this.current.Url}/{v.Path}");
            var covers = files.Where(o => o.ItemType == EItemType.TeamCover).ToDictionary(k => k.ItemId, v => $"{this.current.Url}/{v.Path}");

            var teams = await queryTeams.Select(o => TeamDto.FromEntity(o, avatars.GetValueOrDefault(o.Id), covers.GetValueOrDefault(o.Id), null)).ToListAsync();
            teams = teams.Where(o => o!.Id != req.TeamId).ToList();
            teams = MinderExtension.SortDistance(myTeam!, teams!)!;

            return new ListTeamRes() {
                Count = teams.Count,
                Items = teams.Take(req.Take).Skip(req.Skip).ToList(),
            };
        }

        private static int GetAge(DateTimeOffset dateOfBirth) {
            DateTimeOffset today = DateTimeOffset.Now;
            int age = today.Year - dateOfBirth.Year;

            if (dateOfBirth > today.AddYears(-age)) {
                age--;
            }

            return age;
        }

        private static object GetTime(List<UserDto> members, bool getTimeChooiceModel = false) {
            var sunday = new List<List<int>>();
            var monday = new List<List<int>>();
            var tuesday = new List<List<int>>();
            var wednesday = new List<List<int>>();
            var thursday = new List<List<int>>();
            var friday = new List<List<int>>();
            var saturday = new List<List<int>>();

            foreach (var user in members) {
                sunday.Add(user.GameSetting!.GameTime!.Sunday.Select(o => (int)o).ToList());
                monday.Add(user.GameSetting!.GameTime!.Monday.Select(o => (int)o).ToList());
                tuesday.Add(user.GameSetting!.GameTime!.Tuesday.Select(o => (int)o).ToList());
                wednesday.Add(user.GameSetting!.GameTime!.Wednesday.Select(o => (int)o).ToList());
                thursday.Add(user.GameSetting!.GameTime!.Thursday.Select(o => (int)o).ToList());
                friday.Add(user.GameSetting!.GameTime!.Friday.Select(o => (int)o).ToList());
                saturday.Add(user.GameSetting!.GameTime!.Saturday.Select(o => (int)o).ToList());
            }

            int minLength = 5;
            var gameTime = new GameTimeDto();
            var timeChooices = new List<TimeChooice>();
            for (var i = members.Count; i > 0; i--) {
                var chooices = new TimeChooice[7];
                chooices = FindMostFrequentConsecutiveData(EDayOfWeek.Sunday, sunday, 2, i, chooices);
                chooices = FindMostFrequentConsecutiveData(EDayOfWeek.Monday, monday, 2, i, chooices);
                chooices = FindMostFrequentConsecutiveData(EDayOfWeek.Tuesday, tuesday, 2, i, chooices);
                chooices = FindMostFrequentConsecutiveData(EDayOfWeek.Wednesday, wednesday, 2, i, chooices);
                chooices = FindMostFrequentConsecutiveData(EDayOfWeek.Thursday, thursday, 2, i, chooices);
                chooices = FindMostFrequentConsecutiveData(EDayOfWeek.Friday, friday, 2, i, chooices);
                chooices = FindMostFrequentConsecutiveData(EDayOfWeek.Saturday, saturday, 2, i, chooices);

                if (chooices.ToList().Any(o => o.Length > minLength)) {
                    if (getTimeChooiceModel) {
                        return chooices.ToList();
                    }
                    gameTime = new GameTimeDto() {
                        Sunday = chooices[(int)EDayOfWeek.Sunday].Value.OrderBy(o => o).Select(o => Enum.Parse<EGameTime>(o.ToString())).ToList(),
                        Monday = chooices[(int)EDayOfWeek.Monday].Value.OrderBy(o => o).Select(o => Enum.Parse<EGameTime>(o.ToString())).ToList(),
                        Tuesday = chooices[(int)EDayOfWeek.Tuesday].Value.OrderBy(o => o).Select(o => Enum.Parse<EGameTime>(o.ToString())).ToList(),
                        Wednesday = chooices[(int)EDayOfWeek.Wednesday].Value.OrderBy(o => o).Select(o => Enum.Parse<EGameTime>(o.ToString())).ToList(),
                        Thursday = chooices[(int)EDayOfWeek.Thursday].Value.OrderBy(o => o).Select(o => Enum.Parse<EGameTime>(o.ToString())).ToList(),
                        Friday = chooices[(int)EDayOfWeek.Friday].Value.OrderBy(o => o).Select(o => Enum.Parse<EGameTime>(o.ToString())).ToList(),
                        Saturday = chooices[(int)EDayOfWeek.Saturday].Value.OrderBy(o => o).Select(o => Enum.Parse<EGameTime>(o.ToString())).ToList(),
                    };
                } else if (i == 1) {
                    minLength--;
                    i = members.Count;
                }
            }

            return gameTime;
        }

        public static List<int> FindBestChooice(List<TimeChooice> chooices) {
            TimeChooice? bestObject = null;
            double bestScore = double.MinValue;

            foreach (var chooice in chooices) {
                double score = chooice.Quantity * 9 + chooice.Length * 10;

                if (score > bestScore) {
                    bestObject = chooice;
                    bestScore = score;
                }
            }

            return bestObject?.Value ?? new List<int>();
        }

        private static Point CalculatorCenterPoint(List<UserDto> users) {
            var lat = decimal.Zero;
            var lng = decimal.Zero;
            int nextUser = 0;
            foreach (var user in users) {
                if (user.GameSetting?.Latitude == 0 && user.GameSetting?.Longitude == 0) {
                    nextUser++;
                    continue;
                }
                lat += user.GameSetting?.Latitude ?? decimal.Zero;
                lng += user.GameSetting?.Longitude ?? decimal.Zero;
            }
            var count = users.Count - nextUser;
            return new Point {
                Latitude = count == 0 ? 0 : lat / count,
                Longitude = count == 0 ? 0 : lng / count
            };
        }

        private static TimeChooice[] FindMostFrequentConsecutiveData(EDayOfWeek day, List<List<int>> arrayLists, int minOccurrence, int minArrays, TimeChooice[] chooices) {
            var numberCount = new Dictionary<int, int>();

            foreach (var array in arrayLists) {
                foreach (var number in array) {
                    if (numberCount.ContainsKey(number)) {
                        numberCount[number]++;
                    } else {
                        numberCount[number] = 1;
                    }
                }
            }
            var mostContaint = numberCount.Values.Any() ? numberCount.Values.Max() : 0;
            var mostFrequentConsecutiveData = new List<int>();
            var mostCount = 0;

            var hasValue = ++mostContaint;
            do {
                hasValue--;
                foreach (var kvp in numberCount) {
                    if (kvp.Value >= hasValue && IsConsecutiveSequence(numberCount.Keys, kvp.Key, minArrays, arrayLists.Count)) {
                        mostFrequentConsecutiveData.Add(kvp.Key);
                        mostCount = mostCount > kvp.Value ? mostCount : kvp.Value;
                    }
                }
            } while (!mostFrequentConsecutiveData.Any() && hasValue > minOccurrence);

            chooices[(int)day] = new TimeChooice() {
                Day = day,
                Quantity = mostCount,
                Value = mostFrequentConsecutiveData
            };
            return chooices;
        }

        private static bool IsConsecutiveSequence(IEnumerable<int> numbers, int startNumber, int minArrays, int totalArrays) {
            int currentNumber = startNumber;
            int arrayCount = 0;

            while (numbers.Contains(currentNumber)) {
                currentNumber++;
                arrayCount++;
            }

            return arrayCount >= minArrays && arrayCount <= totalArrays;
        }
    }
}