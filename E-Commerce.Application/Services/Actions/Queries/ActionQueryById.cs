using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.Actions.Queries
{
    public class ActionQueryById
    {
        public class Query : IRequest<BaseResponse<ActionModel>>
        {
            public int ActionId { get; set; }
            public Query(int actionId)
            {
                this.ActionId = actionId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<ActionModel>>
        {
            public async Task<BaseResponse<ActionModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    ActionModel action = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpressionAsNoTracking(p => p.ActionId == request.ActionId).Select(s => new ActionModel()
                    {
                        ActionId = request.ActionId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                    }).FirstOrDefault();
                    return new BaseResponse<ActionModel>(true, string.Empty, string.Empty, action);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ActionQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<ActionModel>(false, ex.Message);
                }
            }
        }
    }
}
