

using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;

namespace E_Commerce.Application.Services.DashBoardManagement.CitiesManagement.Queries
{
    public class CityQueryById
    {
        public class Query : IRequest<BaseResponse<CityModel>>
        {
            public int CityId { get; set; }
            public Query(int cityId)
            {
                CityId = cityId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<CityModel>>
        {
            public async Task<BaseResponse<CityModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    CityModel city = _unitOfWork.GetRepository<City>().GetByExpressionAsNoTracking(p => p.CityId == request.CityId).Select(s => new CityModel()
                    {
                        CityId = s.CityId,
                        CountryId = s.CountryId,
                        Name = s.Name,
                        NameAr = s.NameAr
                    }).FirstOrDefault();
                    return new BaseResponse<CityModel>(true, string.Empty, string.Empty, city);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CityQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<CityModel>(false, ex.Message);
                }
            }
        }
    }
}
