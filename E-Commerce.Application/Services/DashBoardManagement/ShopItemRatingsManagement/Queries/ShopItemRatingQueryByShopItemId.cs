using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
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

namespace E_Commerce.Application.Services.DashBoardManagement.ShopItemRatingsManagement.Queries
{
    public class ShopItemRatingQueryByShopItemId
    {
        public class Query : IRequest<BaseResponse<List<ShopItemRatingModel>>>
        {
            public Guid ShopItemId { get; set; }
            public Query(Guid shopItemId)
            {
                ShopItemId = shopItemId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ShopItemRatingModel>>>
        {
            public async Task<BaseResponse<List<ShopItemRatingModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ShopItemRatingModel> shopItem = _unitOfWork.GetRepository<ShopItemRating>().GetByExpressionAsNoTracking(p => p.ShopItemId == request.ShopItemId).Select(s => new ShopItemRatingModel()
                    {
                        ShopItemRatingId = s.ShopItemRatingId,
                        ShopItemId = s.ShopItemId,
                        Comment = s.Comment,
                        CustomerId = s.CustomerId,
                        Rating = s.Rating,
                        Date = s.CreationDate,
                        Customer = string.Format("{0} {1}", s.Customer.FirstName.Substring(0,1).ToUpper(), s.Customer.LastName.Substring(s.Customer.LastName.Length - 1, 1).ToUpper())
                    }).ToList();

                    return new BaseResponse<List<ShopItemRatingModel>>(true, string.Empty, string.Empty, shopItem);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemRatingQueryByShopItemId", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<ShopItemRatingModel>>(false, ex.Message);
                }
            }
        }
    }
}
