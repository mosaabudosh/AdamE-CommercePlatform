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
    public class ItemCreationCommands
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
                        Name = request.ItemModel.Name.Trim(),
                        NameAr = request.ItemModel.NameAr.Trim(),
                        Image = request.ItemModel.Image?.Trim(),
                        Code = request.ItemModel.Code.Trim(),
                        Descreption = request.ItemModel.Descreption.Trim(),
                        CategoryId = request.ItemModel.CategoryId,
                        IsBlock = false,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(item);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Domain.Domains.Item>().Insert(item);
                    await _unitOfWork.CommitChanges();
                    if (request.ItemModel.FileCreationModel is not null)
                    {
                        FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                        {
                            FolderName = $"{item.ItemId}",
                            FolderPath = $"{request.ItemPathDMS}",
                            FileCreationModels = new List<FileCreationModel>(),
                        };
                        folderAndFileCreationModel.FileCreationModels.Add(request.ItemModel.FileCreationModel);
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
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ItemCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Domain.Domains.Item item)
            {
                try
                {
                    Domain.Domains.Item existingRecord = _unitOfWork.GetRepository<Domain.Domains.Item>().GetByExpressionAsNoTracking(x => x.Name == item.Name || x.NameAr == item.NameAr || x.Code == item.Code).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), item);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
