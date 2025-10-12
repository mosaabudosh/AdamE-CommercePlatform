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

namespace E_Commerce.Application.Services.DashBoardManagement.DiscountManagement.Queries
{
    public class DiscountsQuery
    {
        public class Query : IRequest<BaseResponse<List<DiscountModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<DiscountModel>>>
        {
            public async Task<BaseResponse<List<DiscountModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DiscountModel> actions = _unitOfWork.GetRepository<Discount>().GetByExpressionAsNoTracking(p => (string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)) && !p.IsDeleted).Select(s => new DiscountModel()
                    {
                        DiscountId = s.DiscountId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Code = s.Code,
                        Value = s.Value,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        MaxNumberOfUsage = s.MaxNumberOfUsage,
                        Description = s.Description,
                        FirstValueUse = s.FirstValueUse,
                        Status = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? (s.IsDeleted ? "Deleted" : "Not Deleted") : (s.IsDeleted ? "تم الحذف" : "لم يتم الحذف")
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<DiscountModel>>(true, string.Empty, string.Empty, actions);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "DiscountsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<DiscountModel>>(false, ex.Message);
                }
            }
        }
    }
}
