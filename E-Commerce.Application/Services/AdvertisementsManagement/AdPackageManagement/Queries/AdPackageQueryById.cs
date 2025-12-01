using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Advertisement;
using E_Commerce.Domain.Domains.Advertisements;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.AdvertisementsManagement.AdPackageManagement.Queries
{
    public class AdPackageQueryById
    {
        public class Query : IRequest<BaseResponse<AdPackageModel>>
        {
            public int AdPackageId { get; set; }
            public Query(int adPackageId)
            {
                AdPackageId = adPackageId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<AdPackageModel>>
        {
            public async Task<BaseResponse<AdPackageModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    AdPackageModel adPackage = _unitOfWork.GetRepository<AdPackage>().GetByExpressionAsNoTracking(p => p.AdPackageId == request.AdPackageId).Select(s => new AdPackageModel()
                    {
                        AdPackageId = s.AdPackageId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Description = s.Description,
                        DescriptionAr = s.DescriptionAr,
                        Price = s.Price,
                        DurationDays = s.DurationDays,
                        AdTypeId = s.AdTypeId,
                    }).FirstOrDefault();

                    return new BaseResponse<AdPackageModel>(true, string.Empty, string.Empty, adPackage);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "AdPackageQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<AdPackageModel>(false, ex.Message);
                }
            }
        }
    }
}
