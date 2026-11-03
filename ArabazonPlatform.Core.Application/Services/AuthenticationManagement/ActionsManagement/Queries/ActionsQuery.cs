using Adam.Logging.Interfaces;
using ArabazonPlatform.Generic.Response;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.ActionsManagement.Queries
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
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ActionModel>>>
        {
            public async Task<BaseResponse<List<ActionModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ActionModel> actions = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(s => new ActionModel()
                    {
                        ActionId = s.ActionId,
                        Name = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
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
