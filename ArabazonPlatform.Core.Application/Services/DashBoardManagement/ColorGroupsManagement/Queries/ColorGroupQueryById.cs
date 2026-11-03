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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ColorGroupsManagement.Queries
{
    public class ColorGroupQueryById
    {
        public class Query : IRequest<BaseResponse<ColorGroupModel>>
        {
            public int ColorGroupId { get; set; }
            public Query(int colorGroupId)
            {
                ColorGroupId = colorGroupId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<ColorGroupModel>>
        {
            public async Task<BaseResponse<ColorGroupModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    ColorGroupModel colorGroup = _unitOfWork.GetRepository<ColorGroup>().GetByExpressionAsNoTracking(p => p.ColorGroupId == request.ColorGroupId).Select(s => new ColorGroupModel()
                    {
                        ColorGroupId = request.ColorGroupId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                    }).FirstOrDefault();
                    return new BaseResponse<ColorGroupModel>(true, string.Empty, string.Empty, colorGroup);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ColorGroupQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<ColorGroupModel>(false, ex.Message);
                }
            }
        }
    }
}
