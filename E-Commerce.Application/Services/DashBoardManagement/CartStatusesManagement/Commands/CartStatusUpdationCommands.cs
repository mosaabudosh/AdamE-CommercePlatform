using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
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

namespace E_Commerce.Application.Services.DashBoardManagement.CartStatusesManagement.Commands
{
    public class CartStatusUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CartStatusModel CartStatusModel { get; set; }
            public Command(CartStatusModel cartStatusModel)
            {
                CartStatusModel = cartStatusModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.CartStatus exisitCartStatus = await _unitOfWork.GetRepository<Domain.Domains.CartStatus>().GetById(request.CartStatusModel.CartStatusId);

                    exisitCartStatus.Name = request.CartStatusModel.Name.Trim();
                    exisitCartStatus.NameAr = request.CartStatusModel.NameAr.Trim();
                    exisitCartStatus.Color = request.CartStatusModel.Color;
                    exisitCartStatus.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                    exisitCartStatus.LastUpdatedDate = DateTime.UtcNow;

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitCartStatus);
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
                    await _exceptionService.LogErrorAsync(ex, "CartStatusUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Domain.Domains.CartStatus cartStatus)
            {
                try
                {
                    Domain.Domains.CartStatus existingRecord = _unitOfWork.GetRepository<Domain.Domains.CartStatus>().GetByExpressionAsNoTracking(x => (x.Name == cartStatus.Name || x.NameAr == cartStatus.NameAr) && x.CartStatusId != cartStatus.CartStatusId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
