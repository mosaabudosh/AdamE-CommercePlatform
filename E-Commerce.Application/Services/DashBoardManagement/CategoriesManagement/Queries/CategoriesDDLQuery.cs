using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
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

namespace E_Commerce.Application.Services.DashBoardManagement.CategoriesManagement.Queries
{
    public class CategoriesDDLQuery
    {
        public class Query : IRequest<BaseResponse<List<DDLDataSource>>>
        {
            public string? SearchText { get; set; }
            public bool IsJustParent { get; set; }
            public bool IsJustChild { get; set; }
            public bool IsJustViewInHome { get; set; }
            public int? ParentCategoryId { get; set; }
            public int? GenderId { get; set; }

            public Query(string? searchText = null, bool isJustParent = false, bool isJustChild = false, bool isJustViewInHome = false, int? parentCategoryId = null, int? genderId = null)
            {
                SearchText = searchText;
                IsJustParent = isJustParent;
                IsJustChild = isJustChild;
                IsJustViewInHome = isJustViewInHome;
                ParentCategoryId = parentCategoryId;
                GenderId = genderId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<DDLDataSource>>>
        {
            public async Task<BaseResponse<List<DDLDataSource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DDLDataSource> categories = _unitOfWork.GetRepository<Category>().GetByExpressionAsNoTracking(p => p.IsActive && 
                    (request.IsJustParent ? !p.ParentCategoryId.HasValue : true) && 
                    (request.IsJustChild ? p.ParentCategoryId.HasValue : true) &&
                    (request.IsJustViewInHome ? p.IsViewInHome : true) &&
                    (string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)) && 
                    (!request.ParentCategoryId.HasValue || p.ParentCategoryId == request.ParentCategoryId.Value) && 
                    (!request.GenderId.HasValue || !p.GenderId.HasValue || p.GenderId == request.GenderId)
                    ).Select(p => new DDLDataSource()
                    {
                        Value = p.CategoryId,
                        Label = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.Name : p.NameAr,
                    }).ToList();
                    return new BaseResponse<List<DDLDataSource>>(true, string.Empty, string.Empty, categories);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CategoriesDDLQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
                }
            }
        }
    }
}
