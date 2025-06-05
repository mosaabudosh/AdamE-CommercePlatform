using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.Domains.Authorisation;
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
    public class CartStatusCreationCommands
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
                    Domain.Domains.CartStatus cartStatus = new Domain.Domains.CartStatus()
                    {
                        Name = request.CartStatusModel.Name.Trim(),
                        NameAr = request.CartStatusModel.NameAr.Trim(),
                        Color = request.CartStatusModel.Color,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(cartStatus);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Domain.Domains.CartStatus>().Insert(cartStatus);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartStatusCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Domain.Domains.CartStatus cartStatus)
            {
                try
                {
                    Domain.Domains.CartStatus existingRecord = _unitOfWork.GetRepository<Domain.Domains.CartStatus>().GetByExpressionAsNoTracking(x => x.Name == cartStatus.Name || x.NameAr == cartStatus.NameAr).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), cartStatus);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
