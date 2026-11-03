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
    public class PaymentMethodsQuery
    {
        public class Query : IRequest<BaseResponse<List<PaymentMethodModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<List<PaymentMethodModel>>>
        {
            public async Task<BaseResponse<List<PaymentMethodModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<PaymentMethodModel> paymentMethods = _unitOfWork.GetRepository<PaymentMethod>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(s => new PaymentMethodModel()
                    {
                        PaymentMethodId = s.PaymentMethodId,
                        Name = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                        Code = s.Code,
                        Icon = s.Icon,
                        IsActive = s.IsActive
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (paymentMethods.Any(p => !string.IsNullOrEmpty(p.Icon)))
                    {
                        List<string> filePaths = paymentMethods.Where(p => !string.IsNullOrEmpty(p.Icon)).Select(s => { return $"{_cachingPlatformSettings[PlatformSettingsCode.DMSPaymentMethodPath]}\\_{s.PaymentMethodId}"; }).ToList();
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSPaymentMethodPath, filePaths];
                        if (responseGetFileCreationModels.Any())
                        {
                            foreach (PaymentMethodModel paymentMethod in paymentMethods.Where(p => !string.IsNullOrEmpty(p.Icon)))
                            {
                                paymentMethod.Icon = responseGetFileCreationModels.FirstOrDefault(p => p.FileName == paymentMethod.Icon).FileContent;
                            }
                        }
                    }

                    return new BaseResponse<List<PaymentMethodModel>>(true, string.Empty, string.Empty, paymentMethods);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "PaymentMethodsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<PaymentMethodModel>>(false, ex.Message);
                }
            }
        }
    }
}
