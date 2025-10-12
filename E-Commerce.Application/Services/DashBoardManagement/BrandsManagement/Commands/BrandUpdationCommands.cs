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
    public class BrandUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public BrandModel BrandModel { get; set; }
            public Command(BrandModel brandModel)
            {
                BrandModel = brandModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Brand exisitItem = _unitOfWork.GetRepository<Brand>().GetByExpression(p => p.BrandId == request.BrandModel.BrandId.Value).SingleOrDefault();

                    exisitItem.Name = request.BrandModel.Name.Trim();
                    exisitItem.NameAr = request.BrandModel.NameAr.Trim();
                    exisitItem.Code = request.BrandModel.Code.Trim();
                    exisitItem.Descreption = request.BrandModel.Descreption.Trim();
                    exisitItem.IsBlock = request.BrandModel.IsBlock;
                    exisitItem.IsViewInFooter = request.BrandModel.IsViewInFooter;
                    exisitItem.IsViewInHome = request.BrandModel.IsViewInHome;
                    exisitItem.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                    exisitItem.LastUpdatedDate = DateTime.UtcNow;

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitItem);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    string DMSBrandPath = _cachingPlatformSettings[PlatformSettingsCode.DMSBrandPath];

                    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    {
                        FolderName = $"{exisitItem.BrandId}",
                        FolderPath = $"{DMSBrandPath}",
                        FileCreationModels = new List<FileCreationModel>(),
                    };
                    if(request.BrandModel.FileCreationModel is not null)
                    {
                        folderAndFileCreationModel.FileCreationModels.Add(request.BrandModel.FileCreationModel);
                    }

                    BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                    if (!createFolderAndOneFileResponse.IsSuccess)
                    {
                        return createFolderAndOneFileResponse;
                    }
                    if (request.BrandModel.FileCreationModel is not null)
                    {
                        exisitItem.Image = exisitItem.BrandId.ToString();
                    }
                    else
                    {
                        exisitItem.Image = null;
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
                    await _exceptionService.LogErrorAsync(ex, "BrandUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Brand item)
            {
                try
                {
                    Brand existingRecord = _unitOfWork.GetRepository<Brand>().GetByExpressionAsNoTracking(x => x.Name == item.Name && x.NameAr == item.NameAr && x.Code == item.Code && x.BrandId != item.BrandId).FirstOrDefault();

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
