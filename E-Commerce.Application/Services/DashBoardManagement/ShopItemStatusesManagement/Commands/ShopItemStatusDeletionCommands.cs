using Adam.Logging.Interfaces;
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
    public class ShopItemStatusDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int ShopItemStatusId { get; set; }
            public Command(int shopItemStatusId)
            {
                ShopItemStatusId = shopItemStatusId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.ShopItemStatusId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    ShopItemStatus shopItemStatus = await _unitOfWork.GetRepository<ShopItemStatus>().GetById(request.ShopItemStatusId);
                    _unitOfWork.GetRepository<ShopItemStatus>().Delete(shopItemStatus);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemStatusDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int shopItemStatusId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<ShopItemStatus>().GetByExpressionAsNoTracking(i => i.ShopItemStatusId == shopItemStatusId && i.ShopItems.Any()).Any();
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), shopItemStatusId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
