using Microsoft.EntityFrameworkCore;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.Group;
using Minder.Service.Models.Participant;
using Minder.Services.Common;
using Minder.Services.Extensions;
using Minder.Services.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class GroupService : BaseService, IGroupService {

        public GroupService(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task<string> Create(GroupDto model) {
            await Validate(model);

            if (model.UserIds != null && model.UserIds.Any()) {
                model.Participants ??= new();
                foreach (var userId in model.UserIds) {
                    model.Participants!.Add(new ParticipantDto() {
                        Id = Guid.NewGuid().ToStringN(),
                        UserId = userId
                    });
                }
            } else {
                model.Participants!.Add(new ParticipantDto() {
                    Id = Guid.NewGuid().ToStringN(),
                    UserId = this.current.UserId,
                });
            }

            var group = model.ToEntity();
            await this.db.Groups.AddAsync(group);
            await this.db.SaveChangesAsync();

            return group.Id;
        }

        public async Task Update(GroupDto model) {
            await Validate(model, isCreate: false);

            var entity = await this.db.Groups.FirstOrDefaultAsync(o => o.Id == model.Id);
            await Console.Out.WriteLineAsync(entity?.Id);
            entity!.Title = model.Title;

            await this.db.SaveChangesAsync();
        }

        public Task Delete(string id) {
            throw new NotImplementedException();
        }

        public async Task<GroupDto?> Get(string id) {
            var isExit = await this.db.Groups.AnyAsync(o => o.Id == id);
            ManagedException.ThrowIf(!isExit, Messages.Conversation.Conversation_NotFound);

            var entity = await this.db.Groups.Include(o => o.Messages).AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
            return GroupDto.FromEntity(entity);
        }

        public async Task<ListGroupRes> List(ListGroupReq req) {
            var groupIds = await this.db.Participants.Where(o => o.UserId == this.current.UserId).Select(o => o.GroupId).ToListAsync();
            var query = this.db.Groups.AsNoTracking().Where(o => groupIds.Contains(o.Id));
            var participant = await this.db.Participants.AsNoTracking().Where(o => groupIds.Contains(o.GroupId))
                .GroupBy(o => o.GroupId)
                .Select(o => new {
                    ConversationId = o.Key,
                    ParticipantIds = o.Select(o => o.UserId).ToList()
                }).ToListAsync();

            if (!string.IsNullOrEmpty(req.SearchText)) {
                req.SearchText = req.SearchText.ToLower().ReplaceSpace(isUnsignedUnicode: true);
                query = query.Where(o => o.Title.Contains(req.SearchText) || o.Title.GetSumary().Contains(req.SearchText) || o.Title.ToLower().Contains(req.SearchText));
            }
            var items = await query.OrderBy(o => o.Id).Skip(req.PageIndex * req.PageSize)
                            .Take(req.PageSize).Select(o => GroupDto.FromEntity(o)).ToListAsync();

            foreach (var item in items) {
                if (item == null) continue;
                item.ParticipantIds = participant.FirstOrDefault(o => o.ConversationId == item.Id)?.ParticipantIds;
            }

            return new ListGroupRes() {
                Count = await query.CountIf(req.IsCount, o => o.Id),
                Items = items
            };
        }

        private async Task Validate(GroupDto model, bool isCreate = true) {
            ManagedException.ThrowIf(string.IsNullOrEmpty(model.Title), Messages.Conversation.Conversation_NameRequire);

            if (!isCreate) {
                var isExit = await this.db.Groups.AnyAsync(o => o.Id == model.Id);
                ManagedException.ThrowIf(!isExit, Messages.Conversation.Conversation_NotFound);
            }
        }
    }
}