using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.Generic.Constants.Constants;
using ActionModel = E_Commerce.Application.Model.ActionModel;

namespace E_Commerce.Application.Services.Actions.Commands
{
    public class ActionCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ActionModel ActionModel { get; set; }
            public Command(ActionModel actionModel)
            {
                this.ActionModel = actionModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Authorisation.Action action = new Domain.Domains.Authorisation.Action()
                    {
                        ActionId = 0,
                        Name = request.ActionModel.Name.Trim(),
                        NameAr = !string.IsNullOrEmpty(request.ActionModel.NameAr) ? request.ActionModel.NameAr.Trim() : request.ActionModel.Name.Trim(),
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(action);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().Insert(action);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse<Domain.Domains.Authorisation.Action>(true, string.Empty, action);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ActionCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Domain.Domains.Authorisation.Action action)
            {
                try
                {
                    Domain.Domains.Authorisation.Action existingRecord = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpressionAsNoTracking(x => x.Name == action.Name).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), action);
                    return new BaseResponse<Domain.Domains.Authorisation.Action>(false, ex.Message);
                }
            }
        }
    }
}
