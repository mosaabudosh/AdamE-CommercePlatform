using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.CategoriesManagement.Queries
{
    public class CategoriesQuery
    {
        public class Query : IRequest<BaseResponse<List<CategoryModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<CategoryModel>>>
        {
            public async Task<BaseResponse<List<CategoryModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CategoryModel> categories = _unitOfWork.GetRepository<Domain.Domains.Category>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)).Select(p => new CategoryModel()
                    {
                        CategoryId = p.CategoryId,
                        Name = p.Name,
                        NameAr = p.NameAr,
                        Code = p.Code,
                        Image = p.Image,
                        ParentCategoryId = p.ParentCategoryId,
                        Descreption = p.Descreption,
                        IsBlock = p.IsBlock,
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (categories.Any(p => !string.IsNullOrEmpty(p.Image)))
                    {
                        string DMSCategoryPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCategoryPath];
                        List<string> filePaths = categories.Where(p => !string.IsNullOrEmpty(p.Image)).Select(s => { return $"{DMSCategoryPath}\\{s.Name.Trim()}"; }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<CategoryModel>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (CategoryModel category in categories.Where(p => !string.IsNullOrEmpty(p.Image)))
                            {
                                category.Image = responseGetFileCreationModels.DataObject.FirstOrDefault(p => p.FileName == category.Image).FileContent;
                            }
                        }
                    }

                    return new BaseResponse<List<CategoryModel>>(true, string.Empty, string.Empty, categories);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CategoriesQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<CategoryModel>>(false, ex.Message);
                }
            }
        }
    }
}
