using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Interfaces.DMS;
using E_Commerce.Application.Model;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.Generic.Constants.Constants;

namespace E_Commerce.Application.Services.Categories.Commands
{
    public class CategoryCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CategoryModel CategoryModel { get; set; }

            public string CategoryPathDMS = "Category";

            public Command(CategoryModel categoryModel)
            {
                this.CategoryModel = categoryModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IFolderService _folderService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Category category = new Domain.Domains.Category()
                    {
                        Name = request.CategoryModel.Name.Trim(),
                        NameAr = request.CategoryModel.NameAr.Trim(),
                        Image = request.CategoryModel.Image?.Trim(),
                        Code = request.CategoryModel.Code.Trim(),
                        Descreption = request.CategoryModel.Descreption.Trim(),
                        IsBlock = false,
                        ParentCategoryId = request.CategoryModel.ParentCategoryId,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(category);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    if (request.CategoryModel.FileCreationModel is not null)
                    {
                        FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                        {
                            FolderName = $"{category.CategoryId}",
                            FolderPath = $"{request.CategoryPathDMS}",
                            FileCreationModels = new List<FileCreationModel>(),
                        };
                        folderAndFileCreationModel.FileCreationModels.Add(request.CategoryModel.FileCreationModel);
                        BaseResponse createFolderAndOneFileResponse = await _folderService.CreateFolderAndOneFile(folderAndFileCreationModel);
                        if (!createFolderAndOneFileResponse.IsSuccess)
                        {
                            return createFolderAndOneFileResponse;
                        }
                    }
                    _unitOfWork.GetRepository<Domain.Domains.Category>().Insert(category);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CategoryCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Domain.Domains.Category category)
            {
                try
                {
                    Domain.Domains.Category existingRecord = _unitOfWork.GetRepository<Domain.Domains.Category>().GetByExpressionAsNoTracking(x => x.Name == category.Name || x.Code == category.Code).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), action);
                    return new BaseResponse<Domain.Domains.Authorisation.Action>(false, ex.Message);
                }
            }
        }
    }
}
