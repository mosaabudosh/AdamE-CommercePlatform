using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
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
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.CategoriesManagement.Commands
{
    public class CategoryDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int CategoryId { get; set; }
            public Command(int categoryId)
            {
                CategoryId = categoryId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.CategoryId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }

                    Category category = await _unitOfWork.GetRepository<Category>().GetById(request.CategoryId);
                    _unitOfWork.GetRepository<Category>().Delete(category);

                    if (!string.IsNullOrEmpty(category.Image))
                    {
                        string DMSCategoryPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCategoryPath];
                        string folderPath = $"{DMSCategoryPath}\\{category.Name.Trim()}";
                        BaseResponse responseGetFileCreationModels = await _mediator.Send(new DeleteFolderByPathCommands.Command(folderPath,false));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return responseGetFileCreationModels;
                        }
                    }
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CategoryDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int categoryId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<Category>().GetByExpressionAsNoTracking(i => i.CategoryId == categoryId && (i.Items.Any() || i.ParentCategoryId == categoryId)).Any();
                    if (haveRelatedData)
                    {
                        return new BaseResponse(false, string.Empty, ErrorCode.RelatedData);
                    }
                    return new BaseResponse(true, string.Empty);

                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), categoryId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
