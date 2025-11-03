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

namespace E_Commerce.Application.Services.DashBoardManagement.ShopMenusManagement.Commands
{
    public class ShopMenuDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int ShopMenuId { get; set; }
            public Command(int shopMenuId)
            {
                ShopMenuId = shopMenuId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    ShopMenu shopMenu = await _unitOfWork.GetRepository<ShopMenu>().GetById(request.ShopMenuId);
                    _unitOfWork.GetRepository<ShopMenu>().Delete(shopMenu);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopMenuDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
