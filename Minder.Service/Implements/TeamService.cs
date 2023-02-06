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
using Minder.Services.Models;
using Minder.Services.Resources;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class TeamService : BaseService, ITeamService {

        public TeamService(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task<ListTeamRes> List(BaseListReq req) {
            var teamIds = await this.db.Members.AsNoTracking().Where(o => o.UserId == this.current.UserId).Select(o => o.TeamId).ToListAsync();
            var query = this.db.Teams.AsNoTracking().Where(o => teamIds.Contains(o.Id));

            if (!string.IsNullOrEmpty(req.SearchText)) {
                req.SearchText = req.SearchText.ReplaceSpace(isUnsignedUnicode: true);
                query = query.Where(o => o.Name.Contains(req.SearchText) || o.Name.GetSumary().Contains(req.SearchText) || o.Code.ToLower().Contains(req.SearchText));
            }

            return new ListTeamRes() {
                Count = await query.CountIf(req.IsCount, o => o.Id),
                Items = await query.OrderBy(o => o.Id).Skip(req.PageIndex * req.PageSize).Take(req.PageSize).Select(o => TeamDto.FromEntity(o)).ToListAsync()
            };
        }

        public async Task<TeamDto?> Get(string teamId) {
            var team = await this.db.Teams.FirstOrDefaultAsync(o => o.Id == teamId);
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

            var team = new Team() {
                Id = Guid.NewGuid().ToStringN(),
                Code = model.Code,
                Name = model.Name,
                GameTypes = JsonConvert.SerializeObject(model.GameTypes),
                GameTimes = JsonConvert.SerializeObject(model.GameTimes),
                Longitude = model.Longitude,
                Latitude = model.Latitude,
                Radius = model.Radius,
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
            ManagedException.ThrowIf(!isExitCode, Messages.Team.Team_CodeExited);

            var team = await this.db.Teams.Include(o => o.Members).FirstOrDefaultAsync(o => o.Id == model.Id);
            ManagedException.ThrowIf(team == null, Messages.Team.Team_NotFound);

            team.Code = model.Code;
            team.Name = model.Name;
            team.GameTypes = JsonConvert.SerializeObject(model.GameTypes);
            team.GameTimes = JsonConvert.SerializeObject(model.GameTimes);
            team.Latitude = model.Latitude;
            team.Longitude = model.Longitude;
            team.Radius = model.Radius;

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

        public async Task Delete(string teamId) {
            this.logger.Information($"{nameof(Team)} - {nameof(Delete)} - Start", teamId);

            var isOwner = await this.db.Members.AnyAsync(o => o.UserId == this.current.UserId && o.Regency == ERegency.Owner && o.TeamId == teamId);
            ManagedException.ThrowIf(!isOwner, Messages.Team.Team_NotFound);

            var team = await this.db.Teams.Include(o => o.Members).FirstOrDefaultAsync(o => o.Id == teamId);
            ManagedException.ThrowIf(team == null, Messages.Team.Team_NoPermistion);

            this.db.Teams.Remove(team!);
            await this.db.SaveChangesAsync();
        }
    }
}