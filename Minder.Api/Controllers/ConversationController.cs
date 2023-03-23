using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Interfaces;
using Minder.Service.Models.Conversation;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/conversations")]
    public class ConversationController : ControllerBase {
        public readonly IConversationService conversationService;

        public ConversationController(IConversationService conversationService) {
            this.conversationService = conversationService;
        }

        [HttpGet, Route("{conversationId}")]
        public async Task<BaseRes> Get(string conversationId) {
            try {
                var res = await conversationService.Get(conversationId);
                return BaseRes<ConversationDto?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<BaseRes> List(string? searchText, int pageIndex = 0, int pageSize = 20, bool count = true) {
            try {
                var req = new ListConversationReq() {
                    SearchText = searchText,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    IsCount = count
                };
                var res = await conversationService.List(req);
                return BaseRes<ListConversationRes?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("create")]
        public async Task<BaseRes> Create(ConversationDto model) {
            try {
                var res = await conversationService.Create(model);
                return BaseSaveRes<ConversationDto>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("update")]
        public async Task<BaseRes> Update(ConversationDto model) {
            try {
                await conversationService.Update(model);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}