using Adam.Logging.Interfaces;
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

namespace E_Commerce.Application.Services.DashBoardManagement.OrderStatusesManagement.Commands
{
    public class OrderStatusDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int OrderStatusId { get; set; }
            public Command(int orderStatusId)
            {
                OrderStatusId = orderStatusId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.OrderStatusId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    OrderStatus orderStatus = await _unitOfWork.GetRepository<OrderStatus>().GetById(request.OrderStatusId);
                    _unitOfWork.GetRepository<OrderStatus>().Delete(orderStatus);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "OrderStatusDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int orderStatusId)
            {
                try
                {
                    bool haveUserActions = _unitOfWork.GetRepository<OrderStatus>().GetByExpressionAsNoTracking(i => i.OrderStatusId == orderStatusId && i.Orders.Any()).Any();
                    if (haveUserActions)
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), orderStatusId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
