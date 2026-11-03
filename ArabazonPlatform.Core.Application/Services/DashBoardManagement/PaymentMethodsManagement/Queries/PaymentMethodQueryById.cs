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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.PaymentMethodsManagement.Queries
{
    public class PaymentMethodQueryById
    {
        public class Query : IRequest<BaseResponse<PaymentMethodModel>>
        {
            public int PaymentMethodId { get; set; }
            public Query(int paymentMethodId)
            {
                PaymentMethodId = paymentMethodId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<PaymentMethodModel>>
        {
            public async Task<BaseResponse<PaymentMethodModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    PaymentMethodModel paymentMethod = _unitOfWork.GetRepository<PaymentMethod>().GetByExpressionAsNoTracking(p => p.PaymentMethodId == request.PaymentMethodId).Select(s => new PaymentMethodModel()
                    {
                        PaymentMethodId = request.PaymentMethodId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Code = s.Code,
                        Icon =s.Icon,
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(paymentMethod.Icon))
                    {
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSPaymentMethodPath, $"{_cachingPlatformSettings[PlatformSettingsCode.DMSPaymentMethodPath]}\\_{paymentMethod.PaymentMethodId}"];
                        if (responseGetFileCreationModels.Any())
                        {
                            paymentMethod.FileCreationModel = responseGetFileCreationModels.FirstOrDefault();
                        }
                    }

                    return new BaseResponse<PaymentMethodModel>(true, string.Empty, string.Empty, paymentMethod);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "PaymentMethodQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<PaymentMethodModel>(false, ex.Message);
                }
            }
        }
    }
}
