using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
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
    public class OrderStatusUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public OrderStatusModel OrderStatusModel { get; set; }
            public Command(OrderStatusModel orderStatusModel)
            {
                OrderStatusModel = orderStatusModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.OrderStatus exisitOrderStatus = await _unitOfWork.GetRepository<Domain.Domains.OrderStatus>().GetById(request.OrderStatusModel.OrderStatusId.Value);

                    exisitOrderStatus.Name = request.OrderStatusModel.Name.Trim();
                    exisitOrderStatus.NameAr = request.OrderStatusModel.NameAr.Trim();
                    exisitOrderStatus.Color = request.OrderStatusModel.Color;
                    exisitOrderStatus.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                    exisitOrderStatus.LastUpdatedDate = DateTime.UtcNow;

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitOrderStatus);
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
                    await _exceptionService.LogErrorAsync(ex, "OrderStatusUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Domain.Domains.OrderStatus orderStatus)
            {
                try
                {
                    Domain.Domains.OrderStatus existingRecord = _unitOfWork.GetRepository<Domain.Domains.OrderStatus>().GetByExpressionAsNoTracking(x => (x.Name == orderStatus.Name || x.NameAr == orderStatus.NameAr) && x.OrderStatusId != orderStatus.OrderStatusId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
