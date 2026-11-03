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
using Unit = ArabazonPlatform.Domain.Domains.DashBoard.Unit;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.UnitsManagement.Queries
{
    public class UnitQueryById
    {
        public class Query : IRequest<BaseResponse<UnitModel>>
        {
            public int UnitId { get; set; }
            public Query(int unitId)
            {
                UnitId = unitId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<UnitModel>>
        {
            public async Task<BaseResponse<UnitModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    UnitModel unit = _unitOfWork.GetRepository<Unit>().GetByExpressionAsNoTracking(p => p.UnitId == request.UnitId).Select(s => new UnitModel()
                    {
                        UnitId = request.UnitId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Value = s.Value,
                        UnitGroupId = s.UnitGroupId,
                    }).FirstOrDefault();
                    return new BaseResponse<UnitModel>(true, string.Empty, string.Empty, unit);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UnitQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<UnitModel>(false, ex.Message);
                }
            }
        }
    }
}
