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

namespace E_Commerce.Application.Services.AuthenticationManagement.ActionsManagement.Commands
{
    public class ActionDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int ActionId { get; set; }
            public Command(int actionId)
            {
                ActionId = actionId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.ActionId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    Domain.Domains.Authorisation.Action action = await _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetById(request.ActionId);
                    _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().Delete(action);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ActionDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int actionId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpressionAsNoTracking(i => i.ActionId == actionId && i.ScreenActions.Any()).Any();
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), actionId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
