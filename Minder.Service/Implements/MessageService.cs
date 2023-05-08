using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database.Enums;
using Minder.Service.Interfaces;
using Minder.Service.Models.Message;
using Minder.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class MessageService : BaseService, IMessageService {
        private readonly IChatService chatService;

        public MessageService(IServiceProvider serviceProvider) : base(serviceProvider) {
            chatService = serviceProvider.GetRequiredService<IChatService>();
        }

        public async Task CreateMessage(MessageDto model) {
            var isExit = await this.db.Participants.AnyAsync(o => o.GroupId == model.GroupId && o.UserId == this.current.UserId);
            if (!isExit) return;
            var message = model.ToEntity();
            message.SenderId = this.current.UserId;
            await this.db.Messages.AddAsync(message);
            await this.db.SaveChangesAsync();
            await chatService.SendNotify(model.GroupId, ENotify.RecieveMessage);
        }

        public async Task<ListMessageData> List(ListMessageReq req) {
            if (req.GroupId == null) return new();
            var query = this.db.Messages.Include(o => o.User).AsNoTracking().Where(o => o.GroupId == req.GroupId);
            var messages = await query.OrderByDescending(o => o.CreateAt).Take(req.Take).Skip(req.Skip).ToListAsync();

            var userIds = messages.Select(o => o.SenderId).Distinct().ToList();
            var files = await this.db.Files.Where(o => userIds.Contains(o.ItemId) && o.Type == EFile.Image && o.ItemType == EItemType.UserAvatar)
                .GroupBy(o => o.ItemId).ToDictionaryAsync(k => k.Key, v => v.OrderBy(o => o.UploadDate).FirstOrDefault());

            var messageDtos = messages.OrderBy(o => o.CreateAt).Select(o => MessageDto.FromEntity(o, this.current.UserId, files.GetValueOrDefault(o.SenderId), this.current.Url)).ToList();
            var size = messageDtos.Count;
            for (int i = 0; i < messageDtos.Count; i++) {
                var item = messageDtos[i];

                if (i == 0) {
                    item!.IsDisplayAvatar = !item.IsSend;
                } else if (messageDtos[i - 1]!.SenderId == item!.SenderId) {
                    item.IsDisplayAvatar = false;
                } else item.IsDisplayAvatar = true;

                if (i < size - 1) {
                    item.IsDisplayTime = messageDtos[i + 1]!.User?.Id != item!.User?.Id;
                } else item.IsDisplayTime = true;

                messageDtos[i] = item;
            }

            return new ListMessageData() {
                Count = await query.CountAsync(),
                Items = messageDtos,
            };
        }
    }
}