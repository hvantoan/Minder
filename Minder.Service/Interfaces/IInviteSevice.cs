using Minder.Service.Models.Team;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IInviteSevice {

        Task Create(InviteDto model);

        Task<ListInviteRes> ListInvite(ListInviteReq req);

        Task ConfirmInvite(ConfirmInviteReq req);
    }
}