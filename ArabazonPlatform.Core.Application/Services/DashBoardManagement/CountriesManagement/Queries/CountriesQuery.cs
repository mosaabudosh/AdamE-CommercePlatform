using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.CountriesManagement.Queries
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
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<List<CountryModel>>>
        {
            public async Task<BaseResponse<List<CountryModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CountryModel> countries = _unitOfWork.GetRepository<Country>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(p => new CountryModel()
                    {
                        CountryId = p.CountryId,
                        Name = p.Name,
                        NameAr = p.NameAr,
                        Banner = p.Banner
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (countries.Any(p => !string.IsNullOrEmpty(p.Banner)))
                    {
                        List<string> filePaths = countries.Where(p => !string.IsNullOrEmpty(p.Banner)).Select(s => { return $"{_cachingPlatformSettings[PlatformSettingsCode.DMSCountryPath]}\\{s.Name.Trim()}"; }).ToList();
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSCountryPath, filePaths];
                        if (responseGetFileCreationModels.Any())
                        {
                            foreach (CountryModel country in countries.Where(p => !string.IsNullOrEmpty(p.Banner)))
                            {
                                country.Banner = responseGetFileCreationModels.FirstOrDefault(p => p.FileName == country.Banner)?.FileContent;
                            }
                        }
                    }

                    return new BaseResponse<List<CountryModel>>(true, string.Empty, string.Empty, countries);
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
