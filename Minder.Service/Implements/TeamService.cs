﻿using Microsoft.EntityFrameworkCore;
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
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class TeamService : BaseService, ITeamService {

        public TeamService(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task<ListTeamRes> List(ListTeamReq req) {
            var members = await this.db.Members.AsNoTracking().WhereIf(req.IsMyTeam, o => o.UserId == this.current.UserId).ToListAsync();
            var teamIds = members.Select(o => o.TeamId).ToList();
            var query = this.db.Teams.AsNoTracking().Where(o => teamIds.Contains(o.Id));

            if (!string.IsNullOrEmpty(req.SearchText)) {
                req.SearchText = req.SearchText.ReplaceSpace(isUnsignedUnicode: true);
                query = query.Where(o => o.Name.Contains(req.SearchText) || o.Name.GetSumary().Contains(req.SearchText) || o.Code.ToLower().Contains(req.SearchText));
            }

            var items = await query.OrderBy(o => o.Id).Skip(req.PageIndex * req.PageSize).Take(req.PageSize).Select(o => TeamDto.FromEntity(o)).ToListAsync();
            members = members.Where(o => o.UserId == this.current.UserId).ToList();
            foreach (var item in items) {
                if (item != null) {
                    item.Regency = members.FirstOrDefault(o => o.TeamId == item.Id)?.Regency;
                }
            }

            return new ListTeamRes() {
                Count = await query.CountIf(req.IsCount, o => o.Id),
                Items = items
            };
        }

        public async Task<TeamDto?> Get(string teamId) {
            var team = await this.db.Teams.Include(o => o.GameSetting).Include(o => o.Members).FirstOrDefaultAsync(o => o.Id == teamId);
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

            var isExitCode = await this.db.Teams.AnyAsync(o => o.Code == model.Code);
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
                    GameTime = JsonConvert.SerializeObject(model.GameSetting.GameTime),
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

            var team = await this.db.Teams.Include(o => o.GameSetting).Include(o => o.Members).FirstOrDefaultAsync(o => o.Id == model.Id);
            ManagedException.ThrowIf(team == null, Messages.Team.Team_NotFound);
            var myRegency = team.Members?.FirstOrDefault(o => o.UserId == this.current.UserId);
            ManagedException.ThrowIf(myRegency == null || myRegency.Regency == ERegency.Member, Messages.Team.Team_NoPermistion);

            if (!string.IsNullOrWhiteSpace(model.Code)) team.Code = model.Code;
            if (!string.IsNullOrWhiteSpace(model.Name)) team.Name = model.Name;
            if (model.GameSetting != null) {
                team.GameSetting ??= new();
                team.GameSetting.Id ??= Guid.NewGuid().ToStringN();
                if (model.GameSetting.GameTime != null) team.GameSetting.GameTime = JsonConvert.SerializeObject(model.GameSetting.GameTime);
                if (model.GameSetting.GameTypes != null) team.GameSetting.GameTypes = JsonConvert.SerializeObject(model.GameSetting.GameTypes);
                if (model.GameSetting.Positions != null) team.GameSetting.Positions = JsonConvert.SerializeObject(model.GameSetting.Positions);
                if (model.GameSetting.Longitude != decimal.Zero) team.GameSetting.Longitude = model.GameSetting.Longitude;
                if (model.GameSetting.Latitude != decimal.Zero) team.GameSetting.Latitude = model.GameSetting.Latitude;
                if (model.GameSetting.Radius != 0.0) team.GameSetting.Radius = model.GameSetting.Radius;
                if (model.GameSetting.Rank != ERank.None) team.GameSetting.Rank = model.GameSetting.Rank;
            }

            await this.db.SaveChangesAsync();
            this.logger.Information($"{nameof(Team)} - {nameof(Create)} - End", team.Id);

            return team.Id;
        }

        public async Task Delete(string teamId) {
            this.logger.Information($"{nameof(Team)} - {nameof(Delete)} - Start", teamId);

            var isOwner = await this.db.Members.AnyAsync(o => o.UserId == this.current.UserId && o.Regency == ERegency.Owner && o.TeamId == teamId);
            ManagedException.ThrowIf(!isOwner, Messages.Team.Team_NotFound);

            var team = await this.db.Teams.Include(o => o.Members).Include(o => o.GameSetting).FirstOrDefaultAsync(o => o.Id == teamId);
            ManagedException.ThrowIf(team == null, Messages.Team.Team_NoPermistion);

            this.db.Teams.Remove(team!);
            await this.db.SaveChangesAsync();
        }

        public async Task Invite(InviteDto model) {
            var hasPermisstion = await this.db.Members.AnyAsync(o => o.Regency == ERegency.Owner || o.Regency == ERegency.Captain && o.UserId == this.current.UserId);
            ManagedException.ThrowIf(hasPermisstion, Messages.Team.Team_NoPermistion);
            ManagedException.ThrowIf(model.UserId == this.current.UserId, Messages.Team.Team_NotInviteYourself);
            var invitation = await this.db.Invites.AnyAsync(o => o.TeamId == model.TeamId && o.UserId == model.UserId);
            ManagedException.ThrowIf(invitation, Messages.Invite.Invite_IsExited);

            var invite = new Invitation() {
                Id = Guid.NewGuid().ToStringN(),
                Description = model.Description,
                TeamId = model.TeamId,
                UserId = model.UserId,
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

        public async Task Leave(string teamId) {
            var isInTeam = await this.db.Members.AnyAsync(o => o.TeamId == teamId && o.UserId == this.current.UserId);
            ManagedException.ThrowIf(!isInTeam, Messages.System.System_Error);

            var isOwner = await this.db.Members.AnyAsync(o => o.TeamId == teamId && o.UserId == this.current.UserId && o.Regency == ERegency.Owner);
            ManagedException.ThrowIf(isOwner, Messages.Team.Team_IsOwner);

            var member = await this.db.Members.FirstOrDefaultAsync(o => o.TeamId == teamId && o.UserId == this.current.UserId);
            if (member != null) {
                this.db.Members.Remove(member);
                await this.db.SaveChangesAsync();
            }
        }

        public async Task Kick(string userId) {
            var member = await this.db.Members.AsNoTracking().FirstOrDefaultAsync(o => o.UserId == this.current.UserId && o.Regency == ERegency.Owner);
            ManagedException.ThrowIf(string.IsNullOrEmpty(member?.Id), Messages.Team.Team_NotFound);
            ManagedException.ThrowIf(userId == this.current.UserId, Messages.System.System_Error);

            var memberKick = await this.db.Members.FirstOrDefaultAsync(o => o.TeamId == member.TeamId && o.UserId == userId);
            ManagedException.ThrowIf(memberKick == null, Messages.Team.Team_NotInTeam);

            this.db.Members.Remove(memberKick);
            await this.db.SaveChangesAsync();
        }
    }
}