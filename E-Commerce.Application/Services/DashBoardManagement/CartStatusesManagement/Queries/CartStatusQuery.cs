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

namespace E_Commerce.Application.Services.DashBoardManagement.CartStatusesManagement.Queries
{
    public class CartStatusQuery
    {
        public class Query : IRequest<BaseResponse<List<CartStatusModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<CartStatusModel>>>
        {
            public async Task<BaseResponse<List<CartStatusModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CartStatusModel> cartStatuses = _unitOfWork.GetRepository<CartStatus>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(p => new CartStatusModel()
                    {
                        CartStatusId = p.CartStatusId,
                        Name = p.Name,
                        NameAr = p.NameAr,
                        Color = p.Color
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<CartStatusModel>>(true, string.Empty, string.Empty, cartStatuses);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartStatusQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<CartStatusModel>>(false, ex.Message);
                }
            }
        }
    }
}
