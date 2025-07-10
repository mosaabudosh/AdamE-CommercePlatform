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
    public class ColorsQuery
    {
        public class Query : IRequest<BaseResponse<List<ColorModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ColorModel>>>
        {
            public async Task<BaseResponse<List<ColorModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ColorModel> colors = _unitOfWork.GetRepository<Color>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(s => new ColorModel()
                    {
                        ColorId = s.ColorId,
                        Value = s.Value,
                        ColorGroupId = s.ColorGroupId,
                        Name = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<ColorModel>>(true, string.Empty, string.Empty, colors);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ColorsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<ColorModel>>(false, ex.Message);
                }
            }
        }
    }
}
