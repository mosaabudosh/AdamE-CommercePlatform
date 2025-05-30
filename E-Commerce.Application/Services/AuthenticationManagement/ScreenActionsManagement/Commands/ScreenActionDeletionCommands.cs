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

namespace E_Commerce.Application.Services.AuthenticationManagement.ScreenActionsManagement.Commands
{
    public class ScreenActionDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int ScreenActionId { get; set; }
            public Command(int screenActionId)
            {
                ScreenActionId = screenActionId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Authorisation.ScreenAction screenAction = await _unitOfWork.GetRepository<Domain.Domains.Authorisation.ScreenAction>().GetById(request.ScreenActionId);
                    _unitOfWork.GetRepository<Domain.Domains.Authorisation.ScreenAction>().Delete(screenAction);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ScreenActionDeletionCommands", _userAccessor.GetCurrentUserId(), request.ScreenActionId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
