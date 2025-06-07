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

namespace E_Commerce.Application.Services.DashBoardManagement.CartStatusesManagement.Queries
{
    public class CartStatusQueryById
    {
        public class Query : IRequest<BaseResponse<CartStatusModel>>
        {
            public int CartStatusId { get; set; }
            public Query(int cartStatusId)
            {
                CartStatusId = cartStatusId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<CartStatusModel>>
        {
            public async Task<BaseResponse<CartStatusModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    CartStatusModel cartStatus = _unitOfWork.GetRepository<CartStatus>().GetByExpressionAsNoTracking(p => p.CartStatusId == request.CartStatusId).Select(p => new CartStatusModel()
                    {
                        CartStatusId = p.CartStatusId,
                        Name = p.Name,
                        NameAr = p.NameAr,
                        Color = p.Color,
                    }).FirstOrDefault();
                    return new BaseResponse<CartStatusModel>(true, string.Empty, string.Empty, cartStatus);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartStatusQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<CartStatusModel>(false, ex.Message);
                }
            }
        }
    }
}
