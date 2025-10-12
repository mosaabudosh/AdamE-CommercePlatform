using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System.Xml.Linq;
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.CategoriesManagement.Commands
{
    public class CategoryCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CategoryModel CategoryModel { get; set; }
            public Command(CategoryModel categoryModel)
            {
                CategoryModel = categoryModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                string DMSCategoryPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCategoryPath];
                Category category = new Category();
                try
                {
                    category = new Category()
                    {
                        Name = request.CategoryModel.Name.Trim(),
                        NameAr = request.CategoryModel.NameAr.Trim(),
                        Image = request.CategoryModel.Image?.Trim(),
                        Code = request.CategoryModel.Code.Trim(),
                        Descreption = request.CategoryModel.Descreption.Trim(),
                        IsBlock = false,
                        ParentCategoryId = request.CategoryModel.ParentCategoryId,
                        GenderId = request.CategoryModel.GenderId,
                        IsViewInHome = request.CategoryModel.IsViewInHome,
                        IsViewInFooter = request.CategoryModel.IsViewInFooter,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(category);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Category>().Insert(category);
                    await _unitOfWork.CommitChanges();

                    if (request.CategoryModel.FileCreationModel is not null)
                    {
                        FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                        {
                            FolderName = $"{category.CategoryId.ToString()}",
                            FolderPath = $"{DMSCategoryPath}",
                            FileCreationModels = new List<FileCreationModel>(),
                        };
                        folderAndFileCreationModel.FileCreationModels.Add(request.CategoryModel.FileCreationModel);
                        BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                        if (!createFolderAndOneFileResponse.IsSuccess)
                        {
                            return createFolderAndOneFileResponse;
                        }
                    }
                   
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    if (request.CategoryModel.FileCreationModel is not null)
                    {
                        string folderPath = $"{DMSCategoryPath}\\{category.CategoryId.ToString()}";
                        BaseResponse responseGetFileCreationModels = await _mediator.Send(new DeleteFolderByPathCommands.Command(folderPath));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return responseGetFileCreationModels;
                        }
                    }

                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CategoryCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Category category)
            {
                try
                {
                    Category existingRecord = _unitOfWork.GetRepository<Category>().GetByExpressionAsNoTracking(x => x.Name == category.Name || x.NameAr == category.NameAr || x.Code == category.Code).FirstOrDefault();

                    if (existingRecord is not null)
                    {
                        return new BaseResponse(false, string.Empty, ErrorCode.AlreadyExsist);
                    }
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), category);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
