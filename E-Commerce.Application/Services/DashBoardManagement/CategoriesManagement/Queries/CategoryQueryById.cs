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
    public class CategoryQueryById
    {
        public class Query : IRequest<BaseResponse<CategoryModel>>
        {
            public int CategoryId { get; set; }
            public Query(int categoryId)
            {
                CategoryId = categoryId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<CategoryModel>>
        {
            public async Task<BaseResponse<CategoryModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    CategoryModel category = _unitOfWork.GetRepository<Domain.Domains.Category>().GetByExpressionAsNoTracking(p => p.CategoryId == request.CategoryId).Select(s => new CategoryModel()
                    {
                        CategoryId = s.CategoryId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Code = s.Code,
                        Descreption = s.Descreption,
                        Image = s.Image,
                        IsBlock = s.IsBlock,
                        ParentCategoryId = s.ParentCategoryId,
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(category.Image))
                    {
                        string DMSCategoryPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCategoryPath];
                        string folderPath = $"{DMSCategoryPath}\\{category.Name.Trim()}";
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFolderPathQuery.Query(folderPath));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<CategoryModel>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            category.FileCreationModel = responseGetFileCreationModels.DataObject.FirstOrDefault();
                        }
                    }

                    return new BaseResponse<CategoryModel>(true, string.Empty, string.Empty, category);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CategoryQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<CategoryModel>(false, ex.Message);
                }
            }
        }
    }
}
