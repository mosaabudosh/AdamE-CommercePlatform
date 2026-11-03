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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ItemsManagement.Commands
{
    public class BrandCreationCommands
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
                    Brand brand = new Brand()
                    {
                        Name = request.BrandModel.Name.Trim(),
                        NameAr = request.BrandModel.NameAr.Trim(),
                        Image = request.BrandModel.Image?.Trim(),
                        Code = request.BrandModel.Code.Trim(),
                        Descreption = request.BrandModel.Descreption.Trim(),
                        IsViewInFooter = request.BrandModel.IsViewInFooter,
                        IsViewInHome = request.BrandModel.IsViewInHome,
                        IsBlock = false,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(brand);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Brand>().Insert(brand);
                    await _unitOfWork.CommitChanges();
                    if (request.BrandModel.FileCreationModel is not null)
                    {
                        string DMSBrandPath = _cachingPlatformSettings[PlatformSettingsCode.DMSBrandPath];
                        brand.Image = brand.BrandId.ToString();
                        await _unitOfWork.CommitChanges();
                        FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                        {
                            FolderName = $"{brand.BrandId}",
                            FolderPath = $"{DMSBrandPath}",
                            FileCreationModels = new List<FileCreationModel>(),
                        };
                        folderAndFileCreationModel.FileCreationModels.Add(request.BrandModel.FileCreationModel);
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
                    await _exceptionService.LogErrorAsync(ex, "BrandCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Brand item)
            {
                try
                {
                    Brand existingRecord = _unitOfWork.GetRepository<Brand>().GetByExpressionAsNoTracking(x => x.Name == item.Name || x.NameAr == item.NameAr || x.Code == item.Code).FirstOrDefault();

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
