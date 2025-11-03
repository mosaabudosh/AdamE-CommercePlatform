using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
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
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.ShopCategoriesManagement.Commands
{
    public class ShopCategoryDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int ShopCategoryId { get; set; }
            public Command(int shopCategoryId)
            {
                ShopCategoryId = shopCategoryId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    ShopCategory shopCategory = await _unitOfWork.GetRepository<ShopCategory>().GetById(request.ShopCategoryId);
                    _unitOfWork.GetRepository<ShopCategory>().Delete(shopCategory);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopCategoryDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
          
        }
    }
}
