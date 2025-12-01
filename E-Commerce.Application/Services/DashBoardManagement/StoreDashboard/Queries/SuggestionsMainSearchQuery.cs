using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Settings;
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

namespace E_Commerce.Application.Services.DashBoardManagement.StoreDashboard.Queries
{
    public class SuggestionsMainSearchQuery
    {
        public class Query : IRequest<BaseResponse<List<DDLDataSource>>>
        {
            public string SearchText { get; set; }
            public List<Guid>? NotIncludeIds { get; set; }
            public Query(string searchText, List<Guid>? notIncludeIds = null)
            {
                SearchText = searchText;
                NotIncludeIds = notIncludeIds;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<DDLDataSource>>>
        {
            public async Task<BaseResponse<List<DDLDataSource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DDLDataSource> suggestions = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(p => ((request.NotIncludeIds == null || !request.NotIncludeIds.Any()) || !request.NotIncludeIds.Contains(p.ShopItemId)) && (request.SearchText.Trim().Contains(p.Name) || request.SearchText.Trim().Contains(p.NameAr) || request.SearchText.Trim().Contains(p.SubName) || request.SearchText.Trim().Contains(p.SubNameAr) || request.SearchText.Trim().Contains(p.Shop.Name) || request.SearchText.Trim().Contains(p.Shop.CommercialName) || request.SearchText.Trim().Contains(p.Category.Name) || request.SearchText.Trim().Contains(p.Category.NameAr) || request.SearchText.Trim().Contains(p.Brand.Name) || request.SearchText.Trim().Contains(p.Brand.NameAr) )).Select(s => new DDLDataSource()
                    {
                        ValueGuid = s.ShopItemId,
                        Label = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                        Attributes = new 
                        {
                            ShopId = s.ShopId,
                            CategoryId = s.CategoryId,
                            IsParentCategory = s.Category.ChildCategories.Any(),
                            BrandId = s.BrandId
                        }
                    }).Distinct().Take(100).ToList();

                    return new BaseResponse<List<DDLDataSource>>(true, string.Empty, string.Empty, suggestions);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "SuggestionsMainSearchQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
                }
            }
        }
    }
}
