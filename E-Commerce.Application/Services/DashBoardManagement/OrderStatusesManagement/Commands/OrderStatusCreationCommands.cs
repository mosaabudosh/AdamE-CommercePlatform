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

namespace E_Commerce.Application.Services.DashBoardManagement.OrderStatusesManagement.Commands
{
    public class OrderStatusCreationCommands
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
                    OrderStatus orderStatus = new OrderStatus()
                    {
                        Name = request.OrderStatusModel.Name.Trim(),
                        NameAr = request.OrderStatusModel.NameAr.Trim(),
                        Color = request.OrderStatusModel.Color,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(orderStatus);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<OrderStatus>().Insert(orderStatus);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "OrderStatusCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(OrderStatus orderStatus)
            {
                try
                {
                    OrderStatus existingRecord = _unitOfWork.GetRepository<OrderStatus>().GetByExpressionAsNoTracking(x => x.Name == orderStatus.Name || x.NameAr == orderStatus.NameAr).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), orderStatus);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
