using Adam.Logging.Interfaces;
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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.PaymentMethodsManagement.Commands
{
    public class UpdatePaymentMethodActivationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int PaymentMethodId { get; set; }
            public Command(int paymentMethodId)
            {
                PaymentMethodId = paymentMethodId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    PaymentMethod paymentMethod = await _unitOfWork.GetRepository<PaymentMethod>().GetById(request.PaymentMethodId);
                    paymentMethod.IsActive = !paymentMethod.IsActive;
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UpdatePaymentMethodActivationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
                        
        }
    }
}
