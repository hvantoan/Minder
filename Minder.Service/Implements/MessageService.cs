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
            await SendNotify(model.GroupId, message.Id);
        }

        private async Task SendNotify(string groupId, string id) {
            var entity = await this.db.Messages.Include(o => o.User).AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
            if (entity == null) return;
            var fileEntity = await this.db.Files.FirstOrDefaultAsync(o => o.ItemId == entity.SenderId && o.ItemType == EItemType.UserAvatar);

            var currentMessage = MessageDto.FromEntity(entity, this.current.UserId, fileEntity, this.current.Url);
            var outMessage = new MessageDto() {
                Id = currentMessage.Id,
                GroupId = currentMessage.GroupId,
                Content = currentMessage.Content,
                CreateAt = currentMessage.CreateAt,
                IsDisplayTime = currentMessage.IsDisplayTime,
                IsDisplayAvatar = currentMessage.IsDisplayAvatar,
                IsSend = false,
                MessageType = currentMessage.MessageType,
                SenderId = currentMessage.SenderId,
                User = currentMessage.User
            };
            await chatService.SendMessageNotify(groupId, ENotify.RecieveMessage, outMessage, currentMessage);
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
            if (size == 0) return new();
            var curentDate = messageDtos[0]?.CreateAt.Date ?? DateTimeOffset.UtcNow.Date;

            var response = new List<MessageDto>();

            for (int i = 0; i < messageDtos.Count; i++) {
                var item = messageDtos[i];

                if (item == null) continue;
                if (curentDate != item.CreateAt.Date && i != 0) {
                    var timeLine = new MessageDto() {
                        MessageType = EMessageType.TimeLine,
                        GroupId = item.GroupId,
                        CreateAt = item.CreateAt
                    };
                    curentDate = item.CreateAt.Date;
                    response.Add(timeLine);
                }

                if (i < size - 1) {
                    
                    var nextItemTime = messageDtos[i + 1]!.CreateAt;
                    var itemTime = item.CreateAt;
                    item.IsDisplayTime = !(itemTime.Date == nextItemTime.Date && itemTime.Hour == nextItemTime.Hour && nextItemTime.Minute - itemTime.Minute < 10) 
                        || item.SenderId != messageDtos[i + 1]!.SenderId;
                } else item.IsDisplayTime = true;


                if (i == 0) {
                    item.IsDisplayAvatar = !item.IsSend;
                } else if (messageDtos[i - 1]?.SenderId == item.SenderId) {
                    item.IsDisplayAvatar = false;
                } else {
                    item.IsDisplayAvatar = true;
                };

                response.Add(item);
            }

            return new ListMessageData() {
                Count = await query.CountAsync(),
                Items = response!,
            };
        }
    }
}