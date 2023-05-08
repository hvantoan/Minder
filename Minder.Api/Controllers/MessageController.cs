using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Interfaces;
using Minder.Service.Models.Message;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/messages")]
    public class MessageController : ControllerBase {
        private readonly IMessageService messageService;

        public MessageController(IMessageService messageService) {
            this.messageService = messageService;
        }

        [HttpPost]
        public async Task<BaseRes> List(ListMessageReq req) {
            try {
                var res = await this.messageService.List(req);
                return BaseRes<ListMessageData>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("send-message")]
        public async Task<BaseRes> Send(MessageDto model) {
            try {
                await this.messageService.CreateMessage(model);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}