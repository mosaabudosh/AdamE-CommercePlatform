using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Interfaces.DMS;
using E_Commerce.Application.Model;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.Categories.Queries
{
    internal class CategoryQueryById
    {
        public class Query : IRequest<BaseResponse<CategoryModel>>
        {
            public int CategoryId { get; set; }
            public string CategoryPathDMS = "Category";

            public Query(int categoryId)
            {
                this.CategoryId = categoryId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IFolderService _folderService) : IRequestHandler<Query, BaseResponse<CategoryModel>>
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
                        string folderPath = $"{request.CategoryPathDMS}\\{category.CategoryId}";
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _folderService.GetFilesByFolderPath(folderPath);
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
