using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minder.Service.Implements;
using Minder.Service.Interfaces;
using Minder.Service.Models.Team;
using Minder.Services.Models;
using System;
using System.Threading.Tasks;

namespace Minder.Api.Controllers {

    [ApiController, Authorize, Route("api")]
    public class InviteController : ControllerBase {
        private readonly IInviteSevice inviteSevice;

        public InviteController(IInviteSevice inviteSevice) {
            this.inviteSevice = inviteSevice;
        }
        
        [HttpPost, Route("invites/create")]
        public async Task<BaseRes> Invite(InviteDto model) {
            try {
                await inviteSevice.Invite(model);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("invites")]
        public async Task<BaseRes> ListInvite(ListInviteReq request) {
            try {
                var res = await inviteSevice.ListInvite(request);
                return BaseRes<ListInviteRes>.Ok(res);
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

        [HttpPost, Route("invites/confirm")]
        public async Task<BaseRes> ConfirmInvite(ConfirmInviteReq req) {
            try {
                await inviteSevice.ConfirmInvite(req);
                return BaseRes.Ok();
            } catch (Exception ex) {
                return BaseRes.Fail(ex.Message);
            }
        }

       
    }
}