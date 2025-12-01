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

namespace E_Commerce.Application.Services.AdvertisementsManagement.AdvertisementManagement.Commands
{
    public class AdvertisementDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int AdvertisementId { get; set; }
            public Command(int advertisementId)
            {
                AdvertisementId = advertisementId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.AdvertisementId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    Advertisement advertisement = await _unitOfWork.GetRepository<Advertisement>().GetById(request.AdvertisementId);
                    _unitOfWork.GetRepository<Advertisement>().Delete(advertisement);

                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "AdvertisementDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int advertisementId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<Advertisement>().GetByExpressionAsNoTracking(i => i.AdvertisementId == advertisementId).Any();
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), advertisementId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
