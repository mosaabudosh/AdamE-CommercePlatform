using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.ScreensManagement.Queries
{
    public class UnAssignScreensForRoleQuery
    {
        public class Query : IRequest<BaseResponse<List<AssignUnAssignScreensForRole>>>
        {
            public List<int> ScreenIds { get; set; }
            public Query(List<int> screenIds)
            {
                ScreenIds = screenIds;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<AssignUnAssignScreensForRole>>>
        {
            public async Task<BaseResponse<List<AssignUnAssignScreensForRole>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<AssignUnAssignScreensForRole> unAssignScreensForRoleModels = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Screen>().GetByExpressionAsNoTracking(p => !request.ScreenIds.Contains(p.ScreenId)).Select(s => new AssignUnAssignScreensForRole()
                    {
                        RoleScreenId = 0,
                        RoleId = 0,
                        ScreenId = s.ScreenId,
                        Name = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                        PageActions = s.ScreenActions.Select(a => new RoleScreenActionModel()
                        {
                            RoleScreenActionId = 0,
                            ActionId = a.ActionId,
                            RoleScreenId = 0,
                            ActionName = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? a.Action.Name : a.Action.NameAr,
                            IsChecked = false,
                            ScreenId = s.ScreenId
                        }).ToList()
                    }).ToList();


                    return new BaseResponse<List<AssignUnAssignScreensForRole>>(true, string.Empty, string.Empty, unAssignScreensForRoleModels);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UnAssignScreensForRoleQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<AssignUnAssignScreensForRole>>(false, ex.Message);
                }
            }
        }
    }
}
