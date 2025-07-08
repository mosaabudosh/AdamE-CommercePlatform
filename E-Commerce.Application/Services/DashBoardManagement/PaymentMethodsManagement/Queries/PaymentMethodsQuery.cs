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

namespace E_Commerce.Application.Services.DashBoardManagement.PaymentMethodsManagement.Queries
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<PaymentMethodModel>>>
        {
            public async Task<BaseResponse<List<PaymentMethodModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<PaymentMethodModel> paymentMethods = _unitOfWork.GetRepository<PaymentMethod>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(s => new PaymentMethodModel()
                    {
                        PaymentMethodId = s.PaymentMethodId,
                        Name = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                        Code = s.Code,
                        Icon = s.Icon,
                        IsActive = s.IsActive
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (paymentMethods.Any(p => !string.IsNullOrEmpty(p.Icon)))
                    {
                        string DMSPaymentMethodPath = _cachingPlatformSettings[PlatformSettingsCode.DMSPaymentMethodPath];

                        List<string> filePaths = paymentMethods.Where(p => !string.IsNullOrEmpty(p.Icon)).Select(s => { return $"{DMSPaymentMethodPath}\\_{s.PaymentMethodId}"; }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<PaymentMethodModel>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (PaymentMethodModel paymentMethod in paymentMethods.Where(p => !string.IsNullOrEmpty(p.Icon)))
                            {
                                paymentMethod.Icon = responseGetFileCreationModels.DataObject.FirstOrDefault(p => p.FileName == paymentMethod.Icon).FileContent;
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
