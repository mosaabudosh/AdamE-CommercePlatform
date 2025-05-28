using Adam.Logging.Interfaces;
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

namespace E_Commerce.Application.Services.CartStatuses.Commands
{
    public class CartStatusDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int CartStatusId { get; set; }
            public Command(int cartStatusId)
            {
                this.CartStatusId = cartStatusId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.CartStatusId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    Domain.Domains.CartStatus cartStatus = await _unitOfWork.GetRepository<Domain.Domains.CartStatus>().GetById(request.CartStatusId);
                    _unitOfWork.GetRepository<Domain.Domains.CartStatus>().Delete(cartStatus);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartStatusDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int cartStatusId)
            {
                try
                {
                    bool haveUserActions = _unitOfWork.GetRepository<Domain.Domains.CartStatus>().GetByExpressionAsNoTracking(i => i.CartStatusId == cartStatusId && i.Carts.Any()).Any();
                    if (haveUserActions)
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), cartStatusId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
