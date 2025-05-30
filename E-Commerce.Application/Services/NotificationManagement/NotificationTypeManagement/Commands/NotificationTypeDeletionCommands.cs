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

namespace E_Commerce.Application.Services.NotificationManagement.NotificationTypeManagement.Commands
{
    public class NotificationTypeDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int NotificationTypeId { get; set; }
            public Command(int notificationTypeId)
            {
                this.NotificationTypeId = notificationTypeId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.NotificationTypeId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    Domain.Domains.Notification.NotificationType notificationType = await _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationType>().GetById(request.NotificationTypeId);

                    _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationType>().Delete(notificationType);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationTypeDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int notificationTypeId)
            {
                try
                {
                    bool haveUserActions = _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationType>().GetByExpressionAsNoTracking(i => i.NotificationTypeId == notificationTypeId && i.Notifications.Any()).Any();
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), notificationTypeId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
