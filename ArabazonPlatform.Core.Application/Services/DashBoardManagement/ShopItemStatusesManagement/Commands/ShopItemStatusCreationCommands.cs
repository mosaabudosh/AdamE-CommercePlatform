using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.DashBoard;
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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ShopItemStatusesManagement.Commands
{
    public class ShopItemStatusCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ShopItemStatusModel ShopItemStatusModel { get; set; }
            public Command(ShopItemStatusModel shopItemStatusModel)
            {
                ShopItemStatusModel = shopItemStatusModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    ShopItemStatus shopItemStatus = new ShopItemStatus()
                    {
                        Name = request.ShopItemStatusModel.Name.Trim(),
                        NameAr = !string.IsNullOrEmpty(request.ShopItemStatusModel.NameAr) ? request.ShopItemStatusModel.NameAr.Trim() : request.ShopItemStatusModel.Name.Trim(),
                        Color = request.ShopItemStatusModel.Color,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(shopItemStatus);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<ShopItemStatus>().Insert(shopItemStatus);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemStatusCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(ShopItemStatus shopItemStatus)
            {
                try
                {
                    ShopItemStatus existingRecord = _unitOfWork.GetRepository<ShopItemStatus>().GetByExpressionAsNoTracking(x => x.Name == shopItemStatus.Name || x.NameAr == shopItemStatus.NameAr).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), shopItemStatus);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
