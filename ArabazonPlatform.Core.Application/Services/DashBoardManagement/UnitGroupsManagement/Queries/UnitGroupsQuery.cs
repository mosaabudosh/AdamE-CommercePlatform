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
                        Name = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                        UnitModels = s.Units.Select(u => new UnitModel()
                        {
                            Name = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? u.Name : u.NameAr,
                            UnitId = u.UnitId,
                            UnitGroupId = u.UnitGroupId,
                            Value = u.Value
                        }).ToList()
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
