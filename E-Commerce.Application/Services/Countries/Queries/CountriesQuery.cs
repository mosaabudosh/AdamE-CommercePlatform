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

namespace E_Commerce.Application.Services.Country.Queries
{
    public class CountriesQuery
    {
        public class Query : IRequest<BaseResponse<List<CountryModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<CountryModel>>>
        {
            public async Task<BaseResponse<List<CountryModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CountryModel> cities = _unitOfWork.GetRepository<Domain.Domains.Country>().GetByExpressionAsNoTracking(p => (string.IsNullOrEmpty(request.SearchText) || (p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)))).Select(p => new CountryModel()
                    {
                        CountryId = p.CountryId,
                        Name = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.Name : p.NameAr,
                        Banner = p.Banner
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<CountryModel>>(true, string.Empty, string.Empty, cities);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CountriesQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<CountryModel>>(false, ex.Message);
                }
            }
        }
    }
}
