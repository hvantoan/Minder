using Microsoft.EntityFrameworkCore;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models;
using Minder.Service.Models.Conversation;
using Minder.Services.Common;
using Minder.Services.Extensions;
using Minder.Services.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class ConversationService : BaseService, IConversationService {

        public ConversationService(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task<string> Create(ConversationDto model) {
            await Validate(model);
            var participant = new ParticipantDto() {
                Id = Guid.NewGuid().ToStringN(),
                UserId = this.current.UserId,
            };
            model.Participants!.Add(participant);
            var conversation = model.ToEntity();

            await this.db.Conversations.AddAsync(conversation);
            await this.db.SaveChangesAsync();

            return conversation.Id;
        }

        public async Task Update(ConversationDto model) {
            await Validate(model, isCreate: false);

            var entity = await this.db.Conversations.FirstOrDefaultAsync(o => o.Id == model.Id);
            await Console.Out.WriteLineAsync(entity?.Id);
            entity!.Title = model.Title;

            await this.db.SaveChangesAsync();
        }

        public Task Delete(string id) {
            throw new NotImplementedException();
        }

        public async Task<ConversationDto?> Get(string id) {
            var isExit = await this.db.Conversations.AnyAsync(o => o.Id == id);
            ManagedException.ThrowIf(isExit, Messages.Conversation.Conversation_NotFound);

            var entity = await this.db.Conversations.FirstOrDefaultAsync(o => o.Id == id);
            return ConversationDto.FromEntity(entity);
        }

        public async Task<ListConversationRes> List(ListConversationReq req) {
            var conversationIds = await this.db.Participants.AsNoTracking().Where(o => o.UserId == this.current.UserId).Select(o => o.ConversationId).ToListAsync();
            var query = this.db.Conversations.AsNoTracking().Where(o => conversationIds.Contains(o.Id));

            if (!string.IsNullOrEmpty(req.SearchText)) {
                req.SearchText = req.SearchText.ReplaceSpace(isUnsignedUnicode: true);
                query = query.Where(o => o.Title.Contains(req.SearchText) || o.Title.GetSumary().Contains(req.SearchText) || o.Title.ToLower().Contains(req.SearchText));
            }

            return new ListConversationRes() {
                Count = await query.CountIf(req.IsCount, o => o.Id),
                Items = await query.OrderBy(o => o.Id).Skip(req.PageIndex * req.PageSize)
                            .Take(req.PageSize).Select(o => ConversationDto.FromEntity(o)).ToListAsync()
            };
        }

        private async Task Validate(ConversationDto model, bool isCreate = true) {
            ManagedException.ThrowIf(string.IsNullOrEmpty(model.Title), Messages.Conversation.Conversation_NameRequire);

            if (isCreate) {
                ManagedException.ThrowIf(model.Participants == null || !model.Participants.Any(), Messages.Conversation.Conversation_MinParticipant);
            } else {
                var isExit = await this.db.Conversations.AnyAsync(o => o.Id == model.Id);
                ManagedException.ThrowIf(!isExit, Messages.Conversation.Conversation_NotFound);
            }
        }
    }
}