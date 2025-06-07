using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Domain.Domains.DashBoard;
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

namespace E_Commerce.Application.Services.DashBoardManagement.ShopItemStatusesManagement.Commands
{
    public class ShopItemStatusUpdationCommands
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
                    ShopItemStatus exisitShopItemStatus = await _unitOfWork.GetRepository<ShopItemStatus>().GetById(request.ShopItemStatusModel.ShopItemStatusId);

                    exisitShopItemStatus.Name = request.ShopItemStatusModel.Name.Trim();
                    exisitShopItemStatus.NameAr = !string.IsNullOrEmpty(request.ShopItemStatusModel.NameAr) ? request.ShopItemStatusModel.NameAr.Trim() : request.ShopItemStatusModel.Name.Trim();
                    exisitShopItemStatus.Color = request.ShopItemStatusModel.Color;
                    exisitShopItemStatus.LastUpdatedDate = DateTime.UtcNow;
                    exisitShopItemStatus.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitShopItemStatus);
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
                    await _exceptionService.LogErrorAsync(ex, "ShopItemStatusUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(ShopItemStatus shopItemStatus)
            {
                try
                {
                    ShopItemStatus existingRecord = _unitOfWork.GetRepository<ShopItemStatus>().GetByExpressionAsNoTracking(x => (x.Name == shopItemStatus.Name || x.NameAr == shopItemStatus.NameAr) && x.ShopItemStatusId != shopItemStatus.ShopItemStatusId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), shopItemStatus);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
