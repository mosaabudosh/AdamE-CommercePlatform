using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Advertisement;
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
    public class AdPackageUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public AdPackageModel AdPackageModel { get; set; }
            public Command(AdPackageModel adPackageModel)
            {
                AdPackageModel = adPackageModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    AdPackage exisitAdPackage = await _unitOfWork.GetRepository<AdPackage>().GetById(request.AdPackageModel.AdPackageId);

                    exisitAdPackage.Name = request.AdPackageModel.Name.Trim();
                    exisitAdPackage.NameAr = !string.IsNullOrEmpty(request.AdPackageModel.NameAr) ? request.AdPackageModel.NameAr.Trim() : request.AdPackageModel.Name.Trim();
                    exisitAdPackage.Description = request.AdPackageModel.Description.Trim();
                    exisitAdPackage.DescriptionAr = !string.IsNullOrEmpty(request.AdPackageModel.DescriptionAr) ? request.AdPackageModel.DescriptionAr.Trim() : request.AdPackageModel.Description.Trim();
                    exisitAdPackage.DurationDays = request.AdPackageModel.DurationDays;
                    exisitAdPackage.Price = request.AdPackageModel.Price;
                    exisitAdPackage.AdTypeId = request.AdPackageModel.AdTypeId;
                    exisitAdPackage.LastUpdatedDate = DateTime.UtcNow;
                    exisitAdPackage.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitAdPackage);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
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
                    await _exceptionService.LogErrorAsync(ex, "AdPackageUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(AdPackage adPackage)
            {
                try
                {
                    AdPackage existingRecord = _unitOfWork.GetRepository<AdPackage>().GetByExpressionAsNoTracking(x => (x.Name == adPackage.Name || x.NameAr == adPackage.NameAr) && x.AdPackageId != adPackage.AdPackageId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), adPackage);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
