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

namespace E_Commerce.Application.Services.DashBoardManagement.UnitGroupsManagement.Queries
{
    public class UnitGroupsQuery
    {
        public class Query : IRequest<BaseResponse<List<UnitGroupModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<UnitGroupModel>>>
        {
            public async Task<BaseResponse<List<UnitGroupModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<UnitGroupModel> units = _unitOfWork.GetRepository<UnitGroup>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(s => new UnitGroupModel()
                    {
                        UnitGroupId = s.UnitGroupId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<UnitGroupModel>>(true, string.Empty, string.Empty, units);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UnitGroupsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<UnitGroupModel>>(false, ex.Message);
                }
            }
        }
    }
}
