using E_Commerce.Application.Model;
using E_Commerce.Domain.Domains.Authorisation;
using Generic.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = E_Commerce.Domain.Domains.Authorisation.Action;

namespace E_Commerce.Application.Interfaces
{
    public interface IActionManagementService
    {
        Task<BaseResponse<Action>> AddAction(ActionModel actionModel);
        Task<BaseResponse> UpdateAction(ActionModel actionModel);
        Task<BaseResponse> DeleteAction(int actionId);
        Task<BaseResponse<List<ActionModel>>> GetActions(int pageNumber, int pageSize);
        Task<BaseResponse<int>> GetActionsCount();
        Task<BaseResponse<ActionModel>> GetActionById(int actionId);
    }
}
