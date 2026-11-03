using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Services.AuthenticationManagement.ScreensManagement.Queries;
using ArabazonPlatform.Domain.Domains.Authorisation;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.RoleScreenManagement.Queries
{
    public class AssignUnAssignScreensForRoleByRoleIdQuery
    {
        public class Query : IRequest<BaseResponse<AssignUnAssignScreensForRoleModel>>
        {
            public int RoleId { get; set; }
            public Query(int roleId)
            {
                RoleId = roleId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Query, BaseResponse<AssignUnAssignScreensForRoleModel>>
        {
            public async Task<BaseResponse<AssignUnAssignScreensForRoleModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    AssignUnAssignScreensForRoleModel assignUnAssignScreensForRoleModel = new AssignUnAssignScreensForRoleModel()
                    {
                        AssignScreensForRoleModels = new List<AssignUnAssignScreensForRole>(),
                        UnScreensForRoleModels = new List<AssignUnAssignScreensForRole>()
                    };

                    assignUnAssignScreensForRoleModel.AssignScreensForRoleModels = _unitOfWork.GetRepository<RoleScreen>().GetByExpressionAsNoTracking(p => p.RoleId == request.RoleId).Select(s => new AssignUnAssignScreensForRole()
                    {
                        RoleScreenId = s.RoleScreenId,
                        RoleId = s.RoleId,
                        ScreenId = s.ScreenId,
                        Name = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Screen.Name : s.Screen.NameAr,
                        PageActions = s.Screen.ScreenActions.Where(a => !s.RoleScreenActions.Select(r => r.ActionId).Contains(a.ActionId)).Select(a => new RoleScreenActionModel()
                        {
                            RoleScreenActionId = 0,
                            ActionId = a.ActionId,
                            ScreenId = s.ScreenId,
                            RoleScreenId = s.RoleScreenId,
                            ActionName = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? a.Action.Name : a.Action.NameAr,
                            IsChecked = false
                        }).Union(s.RoleScreenActions.Select(a => new RoleScreenActionModel()
                        {
                            RoleScreenActionId = a.RoleScreenActionId,
                            ActionId = a.ActionId,
                            RoleScreenId = a.RoleScreenId,
                            ScreenId = s.ScreenId,
                            ActionName = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? a.Action.Name : a.Action.NameAr,
                            IsChecked = true
                        })).ToList()
                    }).ToList();

                    List<int> screenIds = assignUnAssignScreensForRoleModel.AssignScreensForRoleModels.Select(s => { return s.ScreenId; }).ToList();
                    BaseResponse<List<AssignUnAssignScreensForRole>> assignUnAssignScreensForRoleResponse = await _mediator.Send(new UnAssignScreensForRoleQuery.Query(screenIds));
                    if (!assignUnAssignScreensForRoleResponse.IsSuccess)
                    {
                        return new BaseResponse<AssignUnAssignScreensForRoleModel>(false, assignUnAssignScreensForRoleResponse.Message, assignUnAssignScreensForRoleResponse.ErrorCode);
                    }
                    assignUnAssignScreensForRoleModel.UnScreensForRoleModels = assignUnAssignScreensForRoleResponse.DataObject;
                    return new BaseResponse<AssignUnAssignScreensForRoleModel>(true, string.Empty, string.Empty, assignUnAssignScreensForRoleModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "AssignUnAssignScreensForRoleByRoleIdQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<AssignUnAssignScreensForRoleModel>(false, ex.Message);
                }
            }
        }
    }
}
