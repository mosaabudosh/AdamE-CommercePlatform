using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using static ArabazonPlatform.Generic.Constants.Constants;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.CategoriesManagement.Commands
{
    public class CategoryUpdationCommands
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
                try
                {
                    Category exisitCategory = await _unitOfWork.GetRepository<Category>().GetById(request.CategoryModel.CategoryId);

                    exisitCategory.Name = request.CategoryModel.Name.Trim();
                    exisitCategory.NameAr = request.CategoryModel.NameAr.Trim();
                    exisitCategory.Image = request.CategoryModel.FileCreationModel is not null ? request.CategoryModel.Image?.Trim() : null;
                    exisitCategory.Code = request.CategoryModel.Code.Trim();
                    exisitCategory.Descreption = request.CategoryModel.Descreption.Trim();
                    exisitCategory.IsBlock = request.CategoryModel.IsBlock.HasValue ? request.CategoryModel.IsBlock.Value : false;
                    exisitCategory.ParentCategoryId = request.CategoryModel.ParentCategoryId;
                    exisitCategory.GenderId = request.CategoryModel.GenderId;
                    exisitCategory.IsViewInHome = request.CategoryModel.IsViewInHome;
                    exisitCategory.IsViewInFooter = request.CategoryModel.IsViewInFooter;
                    exisitCategory.IsBeauty = request.CategoryModel.IsBeauty;
                    exisitCategory.IsKids = request.CategoryModel.IsKids;
                    exisitCategory.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                    exisitCategory.LastUpdatedDate = DateTime.UtcNow;

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitCategory);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    await _unitOfWork.CommitChanges();

                    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    {
                        FolderName = $"{exisitCategory.CategoryId.ToString()}",
                        FolderPath = $"{DMSCategoryPath}",
                        FileCreationModels = new List<FileCreationModel>(),
                    };
                    if (request.CategoryModel.FileCreationModel is not null)
                    {
                        folderAndFileCreationModel.FileCreationModels.Add(request.CategoryModel.FileCreationModel);
                    }
                    BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                    if (!createFolderAndOneFileResponse.IsSuccess)
                    {
                        return createFolderAndOneFileResponse;
                    }
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    if (request.CategoryModel.FileCreationModel is not null)
                    {
                        string folderPath = $"{DMSCategoryPath}\\{request.CategoryModel.CategoryId.ToString()}";
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
                    await _exceptionService.LogErrorAsync(ex, "CategoryUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Category category)
            {
                try
                {
                    Category existingRecord = _unitOfWork.GetRepository<Category>().GetByExpressionAsNoTracking(x => (x.Name == category.Name || x.NameAr == category.NameAr) && x.CategoryId != category.CategoryId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), category);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
