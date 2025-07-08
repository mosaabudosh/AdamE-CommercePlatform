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
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.ItemsManagement.Commands
{
    public class ItemUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ItemModel ItemModel { get; set; }
            public Command(ItemModel itemModel)
            {
                ItemModel = itemModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Item exisitItem = _unitOfWork.GetRepository<Item>().GetByExpression(p => p.ItemId == request.ItemModel.ItemId.Value).SingleOrDefault();

                    exisitItem.Name = request.ItemModel.Name.Trim();
                    exisitItem.NameAr = request.ItemModel.NameAr.Trim();
                    exisitItem.Code = request.ItemModel.Code.Trim();
                    exisitItem.CategoryId = request.ItemModel.CategoryId;
                    exisitItem.Descreption = request.ItemModel.Descreption.Trim();
                    exisitItem.IsBlock = request.ItemModel.IsBlock.HasValue ? request.ItemModel.IsBlock.Value : false;
                    exisitItem.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                    exisitItem.LastUpdatedDate = DateTime.UtcNow;
                    
                    BaseResponse validationBaseResponse = ValidateUpdation(exisitItem);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    string DMSItemPath = _cachingPlatformSettings[PlatformSettingsCode.DMSItemPath];

                    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    {
                        FolderName = $"{exisitItem.ItemId}",
                        FolderPath = $"{DMSItemPath}",
                        FileCreationModels = request.ItemModel.FileCreationModels,
                    };
                  
                    BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                    if (!createFolderAndOneFileResponse.IsSuccess)
                    {
                        return createFolderAndOneFileResponse;
                    }
                    if (request.ItemModel.FileCreationModels is not null && request.ItemModel.FileCreationModels.Any())
                    {
                        exisitItem.Image = exisitItem.ItemId.ToString();
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
                    await _exceptionService.LogErrorAsync(ex, "ItemUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Item item)
            {
                try
                {
                    Item existingRecord = _unitOfWork.GetRepository<Item>().GetByExpressionAsNoTracking(x => x.Name == item.Name && x.NameAr == item.NameAr && x.Code == item.Code && x.ItemId != item.ItemId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), item);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
