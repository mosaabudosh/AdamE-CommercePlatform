using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Generic.Constants.Constants;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ItemsManagement.Commands
{
    public class BrandDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int BrandId { get; set; }
            public Command(int brandId)
            {
                BrandId = brandId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.BrandId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }

                    Brand brand = _unitOfWork.GetRepository<Brand>().GetByExpressionAsNoTracking(p => p.BrandId == request.BrandId).FirstOrDefault();
                    _unitOfWork.GetRepository<Brand>().Delete(brand);

                    string DMSBrandPath = _cachingPlatformSettings[PlatformSettingsCode.DMSBrandPath];
                    string folderPath = $"{DMSBrandPath}\\{brand.BrandId}";
                    BaseResponse responseGetFileCreationModels = await _mediator.Send(new DeleteFolderByPathCommands.Command(folderPath));
                    if (!responseGetFileCreationModels.IsSuccess)
                    {
                        return responseGetFileCreationModels;
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
                    await _exceptionService.LogErrorAsync(ex, "BrandDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int brandId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<Brand>().GetByExpressionAsNoTracking(i => i.BrandId == brandId && i.ShopItems.Any()).Any();
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), brandId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
