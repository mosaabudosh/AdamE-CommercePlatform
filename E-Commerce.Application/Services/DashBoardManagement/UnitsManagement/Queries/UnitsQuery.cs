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
using Unit = E_Commerce.Domain.Domains.DashBoard.Unit;

namespace E_Commerce.Application.Services.DashBoardManagement.UnitsManagement.Queries
{
    public class UnitsQuery
    {
        public class Query : IRequest<BaseResponse<List<UnitModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<UnitModel>>>
        {
            public async Task<BaseResponse<List<UnitModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<UnitModel> units = _unitOfWork.GetRepository<Unit>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(s => new UnitModel()
                    {
                        UnitId = s.UnitId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        UnitGroupId = s.UnitGroupId,
                        Value = s.Value
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<UnitModel>>(true, string.Empty, string.Empty, units);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UnitsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<UnitModel>>(false, ex.Message);
                }
            }
        }
    }
}
