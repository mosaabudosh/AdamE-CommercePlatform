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

namespace E_Commerce.Application.Services.DashBoardManagement.CartsManagement.Commands
{
    public class CartUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CartModel CartModel { get; set; }
            public Command(CartModel cartModel)
            {
                CartModel = cartModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Cart exisitCart = _unitOfWork.GetRepository<Domain.Domains.Cart>().GetByExpression(p => p.CartId == request.CartModel.CartId).SingleOrDefault();

                    exisitCart.CartStatusId = request.CartModel.CartStatusId;
                    exisitCart.TempCustomerId = request.CartModel.TempCustomerId;
                    exisitCart.UserId = request.CartModel.UserId;
                    exisitCart.LastUpdatedDate = DateTime.UtcNow;
                    exisitCart.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
