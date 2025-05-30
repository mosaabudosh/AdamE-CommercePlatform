using Adam.Logging.Interfaces;
using E_Commerce.Domain.Domains.Notification;
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

namespace E_Commerce.Application.Services.NotificationManagement.NotificationStatusManagement.Commands
{
    public class NotificationStatusDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int NotificationStatusId { get; set; }
            public Command(int notificationStatusId)
            {
                this.NotificationStatusId = notificationStatusId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.NotificationStatusId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    Domain.Domains.Notification.NotificationStatus notificationStatus = await _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationStatus>().GetById(request.NotificationStatusId);

                    _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationStatus>().Delete(notificationStatus);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationStatusDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int notificationStatusId)
            {
                try
                {
                    bool haveUserActions = _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationStatus>().GetByExpressionAsNoTracking(i => i.NotificationStatusId == notificationStatusId && i.Notifications.Any()).Any();
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), notificationStatusId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
