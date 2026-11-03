using ArabazonPlatform.Application.Model.Advertisement;
using ArabazonPlatform.Domain.Domains.Advertisements;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArabazonPlatform.Generic.Response;
using Adam.Logging.Interfaces;

namespace ArabazonPlatform.Application.Services.AdvertisementsManagement.AdPackageManagement.Queries
{
    public class AdPackagesQuery
    {
        public class Query : IRequest<BaseResponse<List<AdPackageModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<AdPackageModel>>>
        {
            public async Task<BaseResponse<List<AdPackageModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<AdPackageModel> adPackageModels = _unitOfWork.GetRepository<AdPackage>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(s => new AdPackageModel()
                    {
                        AdPackageId = s.AdPackageId,
                        AdTypeId = s.AdTypeId,
                        AdType = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.AdType.Name : s.AdType.NameAr,
                        Name = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                        Description = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Description : s.DescriptionAr,
                        DurationDays = s.DurationDays,
                        Price = s.Price
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    return new BaseResponse<List<AdPackageModel>>(true, string.Empty, string.Empty, adPackageModels);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "AdPackagesQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<AdPackageModel>>(false, ex.Message);
                }
            }
        }
    }
}
