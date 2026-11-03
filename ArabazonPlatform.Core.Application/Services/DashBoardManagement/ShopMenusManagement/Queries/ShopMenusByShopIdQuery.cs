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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ShopMenusManagement.Queries
{
    public class ShopMenusByShopIdQuery
    {
        public class Query : IRequest<BaseResponse<List<ShopMenuModel>>>
        {
            public int ShopId { get; set; }
            public Query(int shopId)
            {
                ShopId = shopId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ShopMenuModel>>>
        {
            public async Task<BaseResponse<List<ShopMenuModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ShopMenuModel> shopMenus = _unitOfWork.GetRepository<ShopMenu>().GetByExpressionAsNoTracking(p => p.ShopId == request.ShopId).Select(s => new ShopMenuModel()
                    {
                        ShopId = s.ShopId,
                        Shop = s.Shop.Name,
                        Name = s.Name,
                        Code = s.Code,
                        NameAr = s.NameAr,
                        ShopMenuId = s.ShopMenuId
                    }).ToList();

                    return new BaseResponse<List<ShopMenuModel>>(true, string.Empty, string.Empty, shopMenus);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopMenusByShopIdQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<ShopMenuModel>>(false, ex.Message);
                }
            }
        }
    }
}
