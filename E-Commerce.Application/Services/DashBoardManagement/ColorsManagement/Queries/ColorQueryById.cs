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

namespace E_Commerce.Application.Services.DashBoardManagement.ColorsManagement.Queries
{
    public class ColorQueryById
    {
        public class Query : IRequest<BaseResponse<ColorModel>>
        {
            public int ColorId { get; set; }
            public Query(int colorId)
            {
                ColorId = colorId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<ColorModel>>
        {
            public async Task<BaseResponse<ColorModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    ColorModel color = _unitOfWork.GetRepository<Color>().GetByExpressionAsNoTracking(p => p.ColorId == request.ColorId).Select(s => new ColorModel()
                    {
                        ColorId = s.ColorId,
                        ColorGroupId = s.ColorGroupId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Value = s.Value
                    }).FirstOrDefault();
                    return new BaseResponse<ColorModel>(true, string.Empty, string.Empty, color);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ColorQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<ColorModel>(false, ex.Message);
                }
            }
        }
    }
}
