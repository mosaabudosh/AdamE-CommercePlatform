using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Domain.Domains;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.CategoriesManagement.Commands
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
                    Domain.Domains.Category exisitCategory = await _unitOfWork.GetRepository<Domain.Domains.Category>().GetById(request.CategoryModel.CategoryId);

                    exisitCategory.Name = request.CategoryModel.Name.Trim();
                    exisitCategory.NameAr = request.CategoryModel.NameAr.Trim();
                    exisitCategory.Image = request.CategoryModel.Image?.Trim();
                    exisitCategory.Code = request.CategoryModel.Code.Trim();
                    exisitCategory.Descreption = request.CategoryModel.Descreption.Trim();
                    exisitCategory.IsBlock = request.CategoryModel.IsBlock.HasValue ? request.CategoryModel.IsBlock.Value : false;
                    exisitCategory.ParentCategoryId = request.CategoryModel.ParentCategoryId;
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
                        FolderName = $"{exisitCategory.Name.Trim()}",
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
                        string folderPath = $"{DMSCategoryPath}\\{request.CategoryModel.Name.Trim()}";
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

            private BaseResponse ValidateUpdation(Domain.Domains.Category category)
            {
                try
                {
                    Domain.Domains.Category existingRecord = _unitOfWork.GetRepository<Domain.Domains.Category>().GetByExpressionAsNoTracking(x => (x.Name == category.Name || x.NameAr == category.NameAr) && x.CategoryId != category.CategoryId).FirstOrDefault();

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
