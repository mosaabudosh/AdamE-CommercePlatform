using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
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
using static Adam.Generic.Constants.Constants;

namespace E_Commerce.Application.Services.DashBoardManagement.PaymentMethodsManagement.Commands
{
    public class PaymentMethodUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public PaymentMethodModel PaymentMethodModel { get; set; }
            public Command(PaymentMethodModel paymentMethodModel)
            {
                PaymentMethodModel = paymentMethodModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    PaymentMethod exisitRecord = await _unitOfWork.GetRepository<PaymentMethod>().GetById(request.PaymentMethodModel.PaymentMethodId);

                    exisitRecord.Name = request.PaymentMethodModel.Name.Trim();
                    exisitRecord.NameAr = !string.IsNullOrEmpty(request.PaymentMethodModel.NameAr) ? request.PaymentMethodModel.NameAr.Trim() : request.PaymentMethodModel.Name.Trim();
                    exisitRecord.Icon = request.PaymentMethodModel.Icon;
                    exisitRecord.Code = request.PaymentMethodModel.Code;
                    exisitRecord.LastUpdatedDate = DateTime.UtcNow;
                    exisitRecord.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitRecord);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
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
                    await _exceptionService.LogErrorAsync(ex, "PaymentMethodUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(PaymentMethod paymentMethod)
            {
                try
                {
                    PaymentMethod existingRecord = _unitOfWork.GetRepository<PaymentMethod>().GetByExpressionAsNoTracking(x => (x.Name == paymentMethod.Name || x.NameAr == paymentMethod.NameAr) && x.PaymentMethodId != paymentMethod.PaymentMethodId).FirstOrDefault();

                    if (existingRecord is not null)
                    {
                        return new BaseResponse(false, string.Empty, ErrorCode.AlreadyExsist);
                    }
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), paymentMethod);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
