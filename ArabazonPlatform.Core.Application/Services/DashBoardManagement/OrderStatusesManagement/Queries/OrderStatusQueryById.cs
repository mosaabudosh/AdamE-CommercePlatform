using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.DashBoard;
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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.OrderStatusesManagement.Queries
{
    public class OrderStatusQueryById
    {
        public class Query : IRequest<BaseResponse<OrderStatusModel>>
        {
            public int OrderStatusId { get; set; }
            public Query(int orderStatusId)
            {
                OrderStatusId = orderStatusId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<OrderStatusModel>>
        {
            public async Task<BaseResponse<OrderStatusModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    OrderStatusModel orderStatus = _unitOfWork.GetRepository<OrderStatus>().GetByExpressionAsNoTracking(p => p.OrderStatusId == request.OrderStatusId).Select(p => new OrderStatusModel()
                    {
                        OrderStatusId = p.OrderStatusId,
                        Name = p.Name,
                        NameAr = p.NameAr,
                        Color = p.Color,
                    }).FirstOrDefault();
                    return new BaseResponse<OrderStatusModel>(true, string.Empty, string.Empty, orderStatus);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "OrderStatusQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<OrderStatusModel>(false, ex.Message);
                }
            }
        }
    }
}
