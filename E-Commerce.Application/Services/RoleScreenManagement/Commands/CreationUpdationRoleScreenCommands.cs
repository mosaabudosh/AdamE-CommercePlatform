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

namespace E_Commerce.Application.Services.RoleScreenManagement.Commands
{
    public class CreationUpdationRoleScreenCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CreationUpdationRoleScreenModel CreationUpdationRoleScreenModel { get; set; }
            public Command(CreationUpdationRoleScreenModel creationUpdationRoleScreenModel)
            {
                this.CreationUpdationRoleScreenModel = creationUpdationRoleScreenModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    List<Domain.Domains.Authorisation.RoleScreen> removeRoleScreens = new List<Domain.Domains.Authorisation.RoleScreen>();
                    List<Domain.Domains.Authorisation.RoleScreen> removeSavedRoleScreens = new List<Domain.Domains.Authorisation.RoleScreen>();
                    List<Domain.Domains.Authorisation.RoleScreen> newRoleScreens = new List<Domain.Domains.Authorisation.RoleScreen>();

                    List<RoleScreenAction> removeRoleScreenActions = new List<RoleScreenAction>();
                    List<RoleScreenAction> newRoleScreenActions = new List<RoleScreenAction>();

                    if (!request.CreationUpdationRoleScreenModel.AssignScreensForRole.Any())
                    {
                        removeRoleScreens = _unitOfWork.GetRepository<Domain.Domains.Authorisation.RoleScreen>().GetByExpressionAsNoTracking(p => p.RoleId == request.CreationUpdationRoleScreenModel.RoleId).ToList();
                    }

                    List<int> stableRoleScreenIds = request.CreationUpdationRoleScreenModel.AssignScreensForRole.Where(p => p.RoleScreenId != 0).Select(x => x.RoleScreenId).ToList();
                    removeSavedRoleScreens = _unitOfWork.GetRepository<Domain.Domains.Authorisation.RoleScreen>().GetByExpressionAsNoTracking(p => !stableRoleScreenIds.Contains(p.RoleScreenId) && p.RoleId == request.CreationUpdationRoleScreenModel.RoleId).ToList();
                    if (removeSavedRoleScreens.Any())
                    {
                        removeRoleScreens.AddRange(removeSavedRoleScreens);
                    }

                    if (removeRoleScreens.Any())
                    {
                        _unitOfWork.GetRepository<Domain.Domains.Authorisation.RoleScreen>().RemoveRange(removeRoleScreens);
                    }

                    newRoleScreens = request.CreationUpdationRoleScreenModel.AssignScreensForRole.Where(p => p.RoleScreenId == 0).Select(s => new Domain.Domains.Authorisation.RoleScreen()
                    {
                        RoleId = request.CreationUpdationRoleScreenModel.RoleId,
                        ScreenId = s.ScreenId,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true,
                        RoleScreenActions = !s.PageActions.Where(p => p.IsChecked).Any() ? new List<RoleScreenAction>() : s.PageActions.Where(p => p.IsChecked).Select(p => new RoleScreenAction()
                        {
                            ActionId = p.ActionId,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true,
                        }).ToList()
                    }).ToList();

                    if (newRoleScreens.Any())
                    {
                        _unitOfWork.GetRepository<Domain.Domains.Authorisation.RoleScreen>().AddRange(newRoleScreens);
                    }
                    List<int> removeRoleScreenActionIds = request.CreationUpdationRoleScreenModel.AssignScreensForRole.Where(p => p.RoleScreenId != 0).SelectMany(sm => sm.PageActions.Where(p => p.RoleScreenActionId != 0 && !p.IsChecked).Select(s => s.RoleScreenActionId).ToList()).ToList();
                    if (removeRoleScreenActionIds.Any()) 
                    {
                        removeRoleScreenActions = _unitOfWork.GetRepository<RoleScreenAction>().GetByExpressionAsNoTracking(p => removeRoleScreenActionIds.Contains(p.RoleScreenActionId)).ToList();
                        _unitOfWork.GetRepository<RoleScreenAction>().RemoveRange(removeRoleScreenActions);
                    }

                    newRoleScreenActions = request.CreationUpdationRoleScreenModel.AssignScreensForRole.Where(p => p.RoleScreenId != 0).SelectMany(sm => sm.PageActions.Where(p => p.RoleScreenActionId == 0 && p.IsChecked).Select(s => new RoleScreenAction()
                    {
                        RoleScreenId = s.RoleScreenId,
                        ActionId = s.ActionId,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true,
                    }).ToList()).ToList();

                    if (newRoleScreenActions.Any())
                    {
                        _unitOfWork.GetRepository<RoleScreenAction>().AddRange(newRoleScreenActions);
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
                    await _exceptionService.LogErrorAsync(ex, "CreationUpdationRoleScreenCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
