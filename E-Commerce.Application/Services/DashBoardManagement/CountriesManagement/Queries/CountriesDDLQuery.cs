

using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System.Runtime.InteropServices.Marshalling;

namespace E_Commerce.Application.Services.DashBoardManagement.CountriesManagement.Queries
{
    public class CountriesDDLQuery
    {
        public record Query() : IRequest<BaseResponse<List<DDLDataSource>>>
        {
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<DDLDataSource>>>
        {
            public async Task<BaseResponse<List<DDLDataSource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DDLDataSource> countries = _unitOfWork.GetRepository<Country>().GetByExpressionAsNoTracking(p => p.IsActive).Select(p => new DDLDataSource()
                    {
                        Value = p.CountryId,
                        Label = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.Name : p.NameAr,
                        Attributes = new { Banner = p.Banner }
                    }).ToList();

                    if (countries.Any(p => !string.IsNullOrEmpty((p.Attributes as dynamic).Banner)))
                    {
                        string DMSCountryPath = _cachingPlatformSettings[Domain.Enum.Enums.PlatformSettingsCode.DMSCountryPath];
                        List<string> filePaths = countries.Where(p => !string.IsNullOrEmpty((p.Attributes as dynamic).Banner)).Select(s => { return $"{DMSCountryPath}\\{s.Label.Trim()}"; }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<DDLDataSource>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (DDLDataSource country in countries.Where(p => !string.IsNullOrEmpty((p.Attributes as dynamic).Banner)))
                            {
                                country.Attributes = new { Banner = responseGetFileCreationModels.DataObject.FirstOrDefault(s => s.FileName == (country.Attributes as dynamic).Banner)?.FileContent };
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
