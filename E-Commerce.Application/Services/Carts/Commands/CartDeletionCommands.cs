using Adam.Logging.Interfaces;
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
    internal class CartDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int CartId { get; set; }
            public Command(int cartId)
            {
                this.CartId = cartId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Cart cart = await _unitOfWork.GetRepository<Domain.Domains.Cart>().GetById(request.CartId);
                    _unitOfWork.GetRepository<Domain.Domains.Cart>().Delete(cart);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true, string.Empty);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
