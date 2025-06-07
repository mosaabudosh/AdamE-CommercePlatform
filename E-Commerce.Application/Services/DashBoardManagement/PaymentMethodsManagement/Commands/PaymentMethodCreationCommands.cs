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
    public class PaymentMethodCreationCommands
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
                    PaymentMethod paymentMethod = new PaymentMethod()
                    {
                        Name = request.PaymentMethodModel.Name.Trim(),
                        NameAr = !string.IsNullOrEmpty(request.PaymentMethodModel.NameAr) ? request.PaymentMethodModel.NameAr.Trim() : request.PaymentMethodModel.Name.Trim(),
                        Code = request.PaymentMethodModel.Code,
                        Icon = request.PaymentMethodModel.Icon,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(paymentMethod);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<PaymentMethod>().Insert(paymentMethod);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "PaymentMethodCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(PaymentMethod paymentMethod)
            {
                try
                {
                    PaymentMethod existingRecord = _unitOfWork.GetRepository<PaymentMethod>().GetByExpressionAsNoTracking(x => x.Name == paymentMethod.Name || x.NameAr == paymentMethod.NameAr).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), paymentMethod);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
