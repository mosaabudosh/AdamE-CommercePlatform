using ArabazonPlatform.Generic.Generic;
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
    public class AdPackagesDDLQuery
    {
        public record Query() : IRequest<BaseResponse<List<DDLDataSource>>> { }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<DDLDataSource>>>
        {
            public async Task<BaseResponse<List<DDLDataSource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DDLDataSource> adPackages = _unitOfWork.GetRepository<AdPackage>().GetByExpressionAsNoTracking().Select(p => new DDLDataSource()
                    {
                        Value = p.AdPackageId,
                        Label = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? p.Name : p.NameAr,
                    }).ToList();
                    return new BaseResponse<List<DDLDataSource>>(true, string.Empty, string.Empty, adPackages);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "AdPackagesDDLQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
                }
            }
        }
    }
}
