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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.DiscountManagement.Queries
{
    public class DiscountQueryById
    {
        public class Query : IRequest<BaseResponse<DiscountModel>>
        {
            public int DiscountId { get; set; }
            public Query(int discountId)
            {
                DiscountId = discountId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<DiscountModel>>
        {
            public async Task<BaseResponse<DiscountModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    DiscountModel discountModel = _unitOfWork.GetRepository<Discount>().GetByExpressionAsNoTracking(p => p.DiscountId == request.DiscountId).Select(s => new DiscountModel()
                    {
                        DiscountId = request.DiscountId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Code = s.Code,
                        Description = s.Description,
                        EndDate = s.EndDate,
                        FirstValueUse = s.FirstValueUse,
                        MaxNumberOfUsage = s.MaxNumberOfUsage,
                        StartDate = s.StartDate,
                        Value = s.Value
                    }).FirstOrDefault();
                    return new BaseResponse<DiscountModel>(true, string.Empty, string.Empty, discountModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "DiscountQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<DiscountModel>(false, ex.Message);
                }
            }
        }
    }
}
