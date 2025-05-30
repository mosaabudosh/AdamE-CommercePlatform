using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.Enum;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.DashBoardManagement.CitiesManagement.Queries
{
    public class CitiesQuery
    {
        public class Query : IRequest<BaseResponse<List<CityModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }
            public int? CountryId { get; set; }


            public Query(int pageNumber, int pageSize, string? searchText = null, int? countryId = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
                CountryId = countryId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<CityModel>>>
        {
            public async Task<BaseResponse<List<CityModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CityModel> cities = _unitOfWork.GetRepository<Domain.Domains.City>().GetByExpressionAsNoTracking(p => (string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText) || p.Country.Name.Contains(request.SearchText) || p.Country.NameAr.Contains(request.SearchText)) && (!request.CountryId.HasValue || p.CountryId == request.CountryId.Value)).Select(p => new CityModel()
                    {
                        CityId = p.CityId,
                        Name = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.Name : p.NameAr,
                        Country = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.Country.Name : p.Country.NameAr
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<CityModel>>(true, string.Empty, string.Empty, cities);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CitiesQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<CityModel>>(false, ex.Message);
                }
            }
        }
    }
}
