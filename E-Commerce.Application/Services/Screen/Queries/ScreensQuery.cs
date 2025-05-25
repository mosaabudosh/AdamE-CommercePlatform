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

namespace E_Commerce.Application.Services.Screen.Queries
{
    public class ScreensQuery
    {
        public class Query : IRequest<BaseResponse<List<ScreenModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ScreenModel>>>
        {
            public async Task<BaseResponse<List<ScreenModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ScreenModel> cities = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Screen>().GetByExpressionAsNoTracking(p => (string.IsNullOrEmpty(request.SearchText) || (p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText))) ).Select(p => new ScreenModel()
                    {
                        ScreenId = p.ScreenId,
                        Name = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.Name : p.NameAr,
                        ModuleName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.Module.Name : p.Module.NameAr,
                        InMenue = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? (p.IsInMenue ? "Yes" : "No" ) : (p.IsInMenue ? "نعم" : "لا"),
                        Status = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? (p.IsActive ? "Active" : "In Active") : (p.IsActive ? "فعال" : "غير فعال"),
                        ScreenActions = p.ScreenActions.Select(s => new ScreenActionModel()
                        {
                            ScreenActionId = s.ScreenActionId,
                            ScreenId = s.ScreenId,
                            ActionId = s.ActionId,
                            Action = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Action.Name : s.Action.NameAr,
                        }).ToList()
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<ScreenModel>>(true, string.Empty, string.Empty, cities);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ScreensQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<ScreenModel>>(false, ex.Message);
                }
            }
        }
    }
}
