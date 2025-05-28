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
    public class ActionsQuery
    {
        public class Query : IRequest<BaseResponse<List<ActionModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }


            public Query(int pageNumber, int pageSize, string? searchText = null)
            {
                this.PageNumber = pageNumber;
                this.PageSize = pageSize;
                this.SearchText = searchText;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ActionModel>>>
        {
            public async Task<BaseResponse<List<ActionModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ActionModel> actions = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpressionAsNoTracking().Select(s => new ActionModel()
                    {
                        ActionId = s.ActionId,
                        Name = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<ActionModel>>(true, string.Empty, string.Empty, actions);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ActionsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<ActionModel>>(false, ex.Message);
                }
            }
        }
    }
}
