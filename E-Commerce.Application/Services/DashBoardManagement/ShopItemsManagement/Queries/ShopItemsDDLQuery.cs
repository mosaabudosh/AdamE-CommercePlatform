using Adam.Generic.Generic;
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

namespace E_Commerce.Application.Services.DashBoardManagement.ShopItemsManagement.Queries
{
    public class ShopItemsDDLQuery
    {
        public class Query : IRequest<BaseResponse<List<DDLDataSource>>>
        {
            public string? SearchText { get; set; }
            public int? ShopId { get; set; }
            public Query(string? searchText = null, int? shopId = null)
            {
                SearchText = searchText;
                ShopId = shopId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<DDLDataSource>>>
        {
            public async Task<BaseResponse<List<DDLDataSource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DDLDataSource> shopItems = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(p => p.IsActive && (!request.ShopId.HasValue || request.ShopId.Value == p.ShopId) && (string.IsNullOrEmpty(request.SearchText) || p.Item.Name.Contains(request.SearchText) || p.Item.NameAr.Contains(request.SearchText))).Select(p => new DDLDataSource()
                    {
                        ValueGuid = p.ShopItemId,
                        Label = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.Item.Name : p.Item.NameAr,
                    }).ToList();
                    return new BaseResponse<List<DDLDataSource>>(true, string.Empty, string.Empty, shopItems);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemsDDLQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
                }
            }
        }
    }
}
