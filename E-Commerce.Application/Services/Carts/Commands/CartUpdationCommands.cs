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

namespace E_Commerce.Application.Services.Carts.Commands
{
    public class CartUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CartModel CartModel { get; set; }
            public Command(CartModel cartModel)
            {
                this.CartModel = cartModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Cart cart = new Domain.Domains.Cart()
                    {
                        CartId = request.CartModel.CartId,
                        CartStatusId = request.CartModel.CartStatusId,
                        TempCustomerId = request.CartModel.TempCustomerId,
                        UserId = request.CartModel.UserId,
                        LastUpdatedDate = DateTime.UtcNow,
                        LastUpdateUserId = _userAccessor.GetCurrentUserId(),
                    };

                 
                    _unitOfWork.GetRepository<Domain.Domains.Cart>().Update(cart);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true, string.Empty);
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
