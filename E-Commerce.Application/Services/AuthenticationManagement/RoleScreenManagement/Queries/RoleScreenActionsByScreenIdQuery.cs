using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.AuthenticationManagement.ScreensManagement.Queries;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.AuthenticationManagement.RoleScreenManagement.Queries
{
    public class RoleScreenActionsByScreenIdQuery
    {
        public class Query : IRequest<BaseResponse<List<int>>>
        {
            public int ScreenId { get; set; }
            public Query(int screenId)
            {
                ScreenId = screenId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Query, BaseResponse<List<int>>>
        {
            public async Task<BaseResponse<List<int>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<int> actionIds = _unitOfWork.GetRepository<Domain.Domains.Authorisation.RoleScreenAction>().GetByExpressionAsNoTracking(p => p.RoleScreen.ScreenId == request.ScreenId && p.RoleScreen.Role.UserRoles.Any(x => x.UserId == _userAccessor.GetCurrentUserId())).Select(s => s.ActionId).ToList();

                    return new BaseResponse<List<int>>(true, string.Empty, string.Empty, actionIds);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "RoleScreenActionsByScreenIdQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<int>>(false, ex.Message);
                }
            }
        }
    }
}
