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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ShopMenusManagement.Commands
{
    public class ShopMenuCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public List<ShopMenuModel> ShopMenuModels { get; set; }
            public Command(List<ShopMenuModel> shopMenuModels)
            {
                ShopMenuModels = shopMenuModels;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ShopMenu> newShopMenus = new List<ShopMenu>();
                    if (request.ShopMenuModels.Any(p => p.ShopMenuId == 0))
                    {
                        newShopMenus = request.ShopMenuModels.Where(p => p.ShopMenuId == 0).Select(s => new ShopMenu()
                        {
                            ShopId = s.ShopId,
                            Name = s.Name,
                            NameAr = s.NameAr,
                            Code = s.Code,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        }).ToList();

                        _unitOfWork.GetRepository<ShopMenu>().AddRange(newShopMenus);
                        await _unitOfWork.CommitChanges();
                    }

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopMenuCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
