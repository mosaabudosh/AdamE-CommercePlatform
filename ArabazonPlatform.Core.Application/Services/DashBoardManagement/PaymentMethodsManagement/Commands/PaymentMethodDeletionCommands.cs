using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
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
using static ArabazonPlatform.Generic.Constants.Constants;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.PaymentMethodsManagement.Commands
{
    public class PaymentMethodDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int PaymentMethodId { get; set; }
            public Command(int paymentMethodId)
            {
                PaymentMethodId = paymentMethodId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.PaymentMethodId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    PaymentMethod paymentMethod = await _unitOfWork.GetRepository<PaymentMethod>().GetById(request.PaymentMethodId);
                    _unitOfWork.GetRepository<PaymentMethod>().Delete(paymentMethod);

                    if (!string.IsNullOrEmpty(paymentMethod.Icon))
                    {
                        string DMSPaymentMethodPath = _cachingPlatformSettings[PlatformSettingsCode.DMSPaymentMethodPath];
                        string folderPath = $"{DMSPaymentMethodPath}\\_{paymentMethod.PaymentMethodId}";
                        BaseResponse responseGetFileCreationModels = await _mediator.Send(new DeleteFolderByPathCommands.Command(folderPath));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return responseGetFileCreationModels;
                        }
                    }
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "PaymentMethodDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int paymentMethodId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<PaymentMethod>().GetByExpressionAsNoTracking(i => i.PaymentMethodId == paymentMethodId && i.Orders.Any()).Any();
                    if (haveRelatedData)
                    {
                        return new BaseResponse(false, string.Empty, ErrorCode.RelatedData);
                    }
                    return new BaseResponse(true, string.Empty);

                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), paymentMethodId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
