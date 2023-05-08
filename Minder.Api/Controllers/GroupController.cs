using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Interfaces;
using Minder.Service.Models.Group;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api/groups")]
    public class GroupController : ControllerBase {
        public readonly IGroupService groupService;

        public GroupController(IGroupService conversationService) {
            this.groupService = conversationService;
        }

        [HttpGet, Route("{groupId}")]
        public async Task<BaseRes> Get(string groupId) {
            try {
                var res = await groupService.Get(groupId);
                return BaseRes<GroupDto?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpGet]
        public async Task<BaseRes> List(string? searchText, int pageIndex = 0, int pageSize = 20, bool count = true) {
            try {
                var req = new ListGroupReq() {
                    SearchText = searchText,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    IsCount = count
                };
                var res = await groupService.List(req);
                return BaseRes<ListGroupRes?>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("create")]
        public async Task<BaseRes> Create(GroupDto model) {
            try {
                var res = await groupService.Create(model);
                return BaseSaveRes<GroupDto>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("update")]
        public async Task<BaseRes> Update(GroupDto model) {
            try {
                await groupService.Update(model);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }
    }
}