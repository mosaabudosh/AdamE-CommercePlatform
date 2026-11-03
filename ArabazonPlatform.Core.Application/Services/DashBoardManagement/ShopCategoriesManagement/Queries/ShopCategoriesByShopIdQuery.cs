using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
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
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ShopCategoriesManagement.Queries
{
    public class ShopCategoriesByShopIdQuery
    {
        public class Query : IRequest<BaseResponse<List<ShopCategoryModel>>>
        {
            public int ShopId { get; set; }
            public Query(int shopId)
            {
                ShopId = shopId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ShopCategoryModel>>>
        {
            public async Task<BaseResponse<List<ShopCategoryModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ShopCategoryModel> shopCategories = _unitOfWork.GetRepository<ShopCategory>().GetByExpressionAsNoTracking(p => p.ShopId == request.ShopId).Select(s => new ShopCategoryModel()
                    {
                        ShopId = s.ShopId,
                        CategoryId = s.CategoryId,
                        ShopCategoryId = s.ShopCategoryId,
                        Category = s.Category.Name,
                        Shop = s.Shop.Name
                    }).ToList();
                   
                    return new BaseResponse<List<ShopCategoryModel>>(true, string.Empty, string.Empty, shopCategories);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopCategoriesByShopIdQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<ShopCategoryModel>>(false, ex.Message);
                }
            }
        }
    }
}
