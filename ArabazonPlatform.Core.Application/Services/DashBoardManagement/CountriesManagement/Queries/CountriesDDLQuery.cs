

using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System.Runtime.InteropServices.Marshalling;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.CountriesManagement.Queries
{
    public class CountriesDDLQuery
    {
        public record Query() : IRequest<BaseResponse<List<DDLDataSource>>>
        {
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService,CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<List<DDLDataSource>>>
        {
            public async Task<BaseResponse<List<DDLDataSource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DDLDataSource> countries = _unitOfWork.GetRepository<Country>().GetByExpressionAsNoTracking(p => p.IsActive).Select(p => new DDLDataSource()
                    {
                        Value = p.CountryId,
                        Label = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? p.Name : p.NameAr,
                        Attributes = new { Banner = p.Banner }
                    }).ToList();

                    if (countries.Any(p => !string.IsNullOrEmpty((p.Attributes as dynamic).Banner)))
                    {
                        List<string> filePaths = countries.Where(p => !string.IsNullOrEmpty((p.Attributes as dynamic).Banner)).Select(s => { return $"{_cachingPlatformSettings[PlatformSettingsCode.DMSCountryPath]}\\{s.Label.Trim()}"; }).ToList();

                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSCountryPath, filePaths];
                        if (responseGetFileCreationModels.Any())
                        {
                            foreach (DDLDataSource country in countries.Where(p => !string.IsNullOrEmpty((p.Attributes as dynamic).Banner)))
                            {
                                country.Attributes = new { Banner = responseGetFileCreationModels.FirstOrDefault(s => s.FileName == (country.Attributes as dynamic).Banner)?.FileContent };
                            }
                        }
                    }

                    return new BaseResponse<List<DDLDataSource>>(true, string.Empty, string.Empty, countries);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CountriesDDLQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
                }
            }
        }
    }
}
