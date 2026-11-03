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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ShopItemStatusesManagement.Queries
{
    public class ShopItemStatusQueryById
    {
        public class Query : IRequest<BaseResponse<ShopItemStatusModel>>
        {
            public int ShopItemStatusId { get; set; }
            public Query(int shopItemStatusId)
            {
                ShopItemStatusId = shopItemStatusId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<ShopItemStatusModel>>
        {
            public async Task<BaseResponse<ShopItemStatusModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    ShopItemStatusModel shopItemStatus = _unitOfWork.GetRepository<ShopItemStatus>().GetByExpressionAsNoTracking(p => p.ShopItemStatusId == request.ShopItemStatusId).Select(s => new ShopItemStatusModel()
                    {
                        ShopItemStatusId = request.ShopItemStatusId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Color = s.Color,
                    }).FirstOrDefault();
                    return new BaseResponse<ShopItemStatusModel>(true, string.Empty, string.Empty, shopItemStatus);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemStatusQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<ShopItemStatusModel>(false, ex.Message);
                }
            }
        }
    }
}
