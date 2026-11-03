using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.Advertisement;
using ArabazonPlatform.Domain.Domains.Advertisements;
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

namespace ArabazonPlatform.Application.Services.AdvertisementsManagement.AdPackageManagement.Commands
{
    public class AdPackageCreationCommands
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
                    AdPackage adPackage = new AdPackage()
                    {
                        AdPackageId = 0,
                        Name = request.AdPackageModel.Name.Trim(),
                        NameAr = !string.IsNullOrEmpty(request.AdPackageModel.NameAr) ? request.AdPackageModel.NameAr.Trim() : request.AdPackageModel.Name.Trim(),
                        Description = request.AdPackageModel.Description.Trim(),
                        DescriptionAr = !string.IsNullOrEmpty(request.AdPackageModel.DescriptionAr) ? request.AdPackageModel.DescriptionAr.Trim() : request.AdPackageModel.Description.Trim(),
                        Price = request.AdPackageModel.Price,
                        DurationDays = request.AdPackageModel.DurationDays,
                        AdTypeId = request.AdPackageModel.AdTypeId,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(adPackage);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<AdPackage>().Insert(adPackage);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "AdPackageCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(AdPackage adPackage)
            {
                try
                {
                    AdPackage existingRecord = _unitOfWork.GetRepository<AdPackage>().GetByExpressionAsNoTracking(x => x.Name == adPackage.Name || x.NameAr == adPackage.NameAr).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), adPackage);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
