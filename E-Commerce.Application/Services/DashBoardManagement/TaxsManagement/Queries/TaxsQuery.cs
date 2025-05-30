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

namespace E_Commerce.Application.Services.DashBoardManagement.TaxsManagement.Queries
{
    public class TaxsQuery
    {
        public class Query : IRequest<BaseResponse<List<TaxModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<TaxModel>>>
        {
            public async Task<BaseResponse<List<TaxModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<TaxModel> taxs = _unitOfWork.GetRepository<Domain.Domains.Tax>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(s => new TaxModel()
                    {
                        TaxId = s.TaxId,
                        Name = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                        Code = s.Code
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<TaxModel>>(true, string.Empty, string.Empty, taxs);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "TaxsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<TaxModel>>(false, ex.Message);
                }
            }
        }
    }
}
