using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using static Adam.Generic.Constants.Constants;

namespace E_Commerce.Application.Services.DashBoardManagement.ItemsManagement.Commands
{
    public class ItemUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ItemModel ItemModel { get; set; }

            public string ItemPathDMS = "Items";

            public Command(ItemModel itemModel)
            {
                ItemModel = itemModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Item item = new Domain.Domains.Item()
                    {
                        ItemId = request.ItemModel.ItemId.Value,
                        Name = request.ItemModel.Name.Trim(),
                        NameAr = request.ItemModel.NameAr.Trim(),
                        Image = request.ItemModel.Image?.Trim(),
                        Code = request.ItemModel.Code.Trim(),
                        CategoryId = request.ItemModel.CategoryId,
                        Descreption = request.ItemModel.Descreption.Trim(),
                        IsBlock = request.ItemModel.IsBlock.HasValue ? request.ItemModel.IsBlock.Value : false,
                        LastUpdateUserId = _userAccessor.GetCurrentUserId(),
                        LastUpdatedDate = DateTime.UtcNow,
                    };
                    BaseResponse validationBaseResponse = ValidateUpdation(item);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    {
                        FolderName = $"{item.ItemId}",
                        FolderPath = $"{request.ItemPathDMS}",
                        FileCreationModels = new List<FileCreationModel>(),
                    };
                    if (request.ItemModel.FileCreationModel is not null)
                    {
                        folderAndFileCreationModel.FileCreationModels.Add(request.ItemModel.FileCreationModel);
                    }
                    BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                    if (!createFolderAndOneFileResponse.IsSuccess)
                    {
                        return createFolderAndOneFileResponse;
                    }
                    _unitOfWork.GetRepository<Domain.Domains.Item>().Update(item);
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

            private BaseResponse ValidateUpdation(Domain.Domains.Item item)
            {
                try
                {
                    Domain.Domains.Item existingRecord = _unitOfWork.GetRepository<Domain.Domains.Item>().GetByExpressionAsNoTracking(x => (x.Name == item.Name || x.NameAr == item.NameAr) && x.ItemId != item.ItemId).FirstOrDefault();

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
