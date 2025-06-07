using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
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

namespace E_Commerce.Application.Services.DashBoardManagement.ShopItemStatusesManagement.Queries
{
    public class ShopItemStatusesQuery
    {
        public class Query : IRequest<BaseResponse<List<ShopItemStatusModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }


            public Query(int pageNumber, int pageSize, string? searchText = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ShopItemStatusModel>>>
        {
            public async Task<BaseResponse<List<ShopItemStatusModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ShopItemStatusModel> shopItemStatuses = _unitOfWork.GetRepository<ShopItemStatus>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(s => new ShopItemStatusModel()
                    {
                        ShopItemStatusId = s.ShopItemStatusId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Color = s.Color,
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<ShopItemStatusModel>>(true, string.Empty, string.Empty, shopItemStatuses);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemStatussQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<ShopItemStatusModel>>(false, ex.Message);
                }
            }
        }
    }
}
