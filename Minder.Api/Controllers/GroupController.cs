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

        [HttpPost]
        public async Task<BaseRes> List(ListGroupReq req) {
            try {
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
        public async Task<BaseRes> Update(UpdateGroupReq req) {
            try {
                await groupService.Update(req);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

       
    }
}