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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<CountryModel>>>
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
                        string DMSCountryPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCountryPath];

                        List<string> filePaths = countries.Where(p => !string.IsNullOrEmpty(p.Banner)).Select(s => { return $"{DMSCountryPath}\\{s.Name.Trim()}"; }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<CountryModel>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (CountryModel country in countries.Where(p => !string.IsNullOrEmpty(p.Banner)))
                            {
                                country.Banner = responseGetFileCreationModels.DataObject.FirstOrDefault(p => p.FileName == country.Banner)?.FileContent;
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
