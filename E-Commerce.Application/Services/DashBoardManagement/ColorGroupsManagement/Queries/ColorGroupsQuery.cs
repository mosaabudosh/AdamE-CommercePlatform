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

namespace E_Commerce.Application.Services.DashBoardManagement.ColorGroupsManagement.Queries
{
    public class ColorGroupsQuery
    {
        public class Query : IRequest<BaseResponse<List<ColorGroupModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ColorGroupModel>>>
        {
            public async Task<BaseResponse<List<ColorGroupModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ColorGroupModel> colorGroups = _unitOfWork.GetRepository<ColorGroup>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(s => new ColorGroupModel()
                    {
                        ColorGroupId = s.ColorGroupId,
                        Name = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                        ColorModels = s.Colors.Select(u => new ColorModel()
                        {
                            Name = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? u.Name : u.NameAr,
                            ColorId = u.ColorId,
                            ColorGroupId = u.ColorGroupId,
                            Value = u.Value
                        }).ToList()
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<ColorGroupModel>>(true, string.Empty, string.Empty, colorGroups);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ColorGroupsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<ColorGroupModel>>(false, ex.Message);
                }
            }
        }
    }
}
