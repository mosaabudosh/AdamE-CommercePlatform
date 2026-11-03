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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.UnitGroupsManagement.Queries
{
    public class UnitGroupQueryById
    {
        public class Query : IRequest<BaseResponse<UnitGroupModel>>
        {
            public int UnitGroupId { get; set; }
            public Query(int unitGroupId)
            {
                UnitGroupId = unitGroupId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<UnitGroupModel>>
        {
            public async Task<BaseResponse<UnitGroupModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    UnitGroupModel unitGroup = _unitOfWork.GetRepository<UnitGroup>().GetByExpressionAsNoTracking(p => p.UnitGroupId == request.UnitGroupId).Select(s => new UnitGroupModel()
                    {
                        UnitGroupId = request.UnitGroupId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                    }).FirstOrDefault();
                    return new BaseResponse<UnitGroupModel>(true, string.Empty, string.Empty, unitGroup);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UnitGroupQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<UnitGroupModel>(false, ex.Message);
                }
            }
        }
    }
}
