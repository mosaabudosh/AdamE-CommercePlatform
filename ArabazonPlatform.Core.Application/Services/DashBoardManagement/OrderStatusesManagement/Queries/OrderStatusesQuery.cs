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
    public class OrderStatusesQuery
    {
        public class Query : IRequest<BaseResponse<List<OrderStatusModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<OrderStatusModel>>>
        {
            public async Task<BaseResponse<List<OrderStatusModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<OrderStatusModel> orderStatuses = _unitOfWork.GetRepository<OrderStatus>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(p => new OrderStatusModel()
                    {
                        OrderStatusId = p.OrderStatusId,
                        Name = p.Name ,
                        NameAr = p.NameAr,
                        Color = p.Color
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<OrderStatusModel>>(true, string.Empty, string.Empty, orderStatuses);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "OrderStatusesQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<OrderStatusModel>>(false, ex.Message);
                }
            }
        }
    }
}
