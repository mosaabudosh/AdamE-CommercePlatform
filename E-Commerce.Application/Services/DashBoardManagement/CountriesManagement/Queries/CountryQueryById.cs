using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.CountriesManagement.Queries
{
    public class CountryQueryById
    {
        public class Query : IRequest<BaseResponse<CountryModel>>
        {
            public int CountryId { get; set; }
            public Query(int countryId)
            {
                CountryId = countryId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<CountryModel>>
        {
            public async Task<BaseResponse<CountryModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    CountryModel countryModel = _unitOfWork.GetRepository<Country>().GetByExpressionAsNoTracking(p => p.CountryId == request.CountryId).Select(s => new CountryModel()
                    {
                        CountryId = s.CountryId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Banner = s.Banner
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(countryModel.Banner))
                    {
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSCountryPath, $"{_cachingPlatformSettings[PlatformSettingsCode.DMSCountryPath]}\\{countryModel.Name.Trim()}"];
                        if (responseGetFileCreationModels.Any())
                        {
                            countryModel.FileCreationModel = responseGetFileCreationModels.FirstOrDefault();
                        }
                    }
                    return new BaseResponse<CountryModel>(true, string.Empty, string.Empty, countryModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CountryQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<CountryModel>(false, ex.Message);
                }
            }
        }
    }
}
