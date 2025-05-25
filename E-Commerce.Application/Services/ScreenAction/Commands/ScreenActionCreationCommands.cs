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

namespace E_Commerce.Application.Services.ScreenAction.Commands
{
    public class ScreenActionCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public List<ScreenActionModel> ScreenActions { get; set; }
            public Command(List<ScreenActionModel> screenActions)
            {
                this.ScreenActions = screenActions;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!request.ScreenActions.Any())
                    {
                        return new BaseResponse(true);
                    }

                    List<Domain.Domains.Authorisation.ScreenAction> screenActions = request.ScreenActions.Select(s =>
                    new Domain.Domains.Authorisation.ScreenAction
                    {
                        ScreenActionId = 0,
                        ScreenId = s.ScreenId,
                        ActionId = s.ActionId,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    }).ToList();
                    _unitOfWork.GetRepository<Domain.Domains.Authorisation.ScreenAction>().AddRange(screenActions);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ScreenActionCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
