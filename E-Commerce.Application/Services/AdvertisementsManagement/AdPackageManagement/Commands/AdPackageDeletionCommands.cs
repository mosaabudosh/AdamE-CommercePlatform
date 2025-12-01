using Adam.Logging.Interfaces;
using E_Commerce.Domain.Domains.Advertisements;
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

namespace E_Commerce.Application.Services.AdvertisementsManagement.AdPackageManagement.Commands
{
    public class AdPackageDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int AdPackageId { get; set; }
            public Command(int adPackageId)
            {
                AdPackageId = adPackageId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.AdPackageId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    AdPackage adPackage = await _unitOfWork.GetRepository<AdPackage>().GetById(request.AdPackageId);
                    _unitOfWork.GetRepository<AdPackage>().Delete(adPackage);

                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "AdPackageDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int adPackageId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<AdPackage>().GetByExpressionAsNoTracking(i => i.AdPackageId == adPackageId && i.ShopAdPackages.Any()).Any();
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), adPackageId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
