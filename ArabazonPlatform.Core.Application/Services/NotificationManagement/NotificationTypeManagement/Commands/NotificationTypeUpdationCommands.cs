using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.Notification;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Generic.Constants.Constants;

namespace ArabazonPlatform.Application.Services.NotificationManagement.NotificationTypeManagement.Commands
{
    public class NotificationTypeUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public NotificationTypeModel NotificationTypeModel { get; set; }
            public Command(NotificationTypeModel notificationTypeModel)
            {
                this.NotificationTypeModel = notificationTypeModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Notification.NotificationType exisitNotificationType = await _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationType>().GetById(request.NotificationTypeModel.NotificationTypeId);

                    exisitNotificationType.Name = request.NotificationTypeModel.Name.Trim();
                    exisitNotificationType.NameAr = request.NotificationTypeModel.NameAr.Trim();
                    exisitNotificationType.Icon = request.NotificationTypeModel.Icon;
                    exisitNotificationType.IsPlaySound = request.NotificationTypeModel.IsPlaySound;
                    exisitNotificationType.Code = request.NotificationTypeModel.Code;
                    exisitNotificationType.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                    exisitNotificationType.LastUpdatedDate = DateTime.UtcNow;
                    
                    BaseResponse validationBaseResponse = ValidateUpdation(exisitNotificationType);
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
                    await _exceptionService.LogErrorAsync(ex, "NotificationTypeUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Domain.Domains.Notification.NotificationType notificationStatus)
            {
                try
                {
                    Domain.Domains.Notification.NotificationType existingRecord = _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationType>().GetByExpressionAsNoTracking(x => (x.Name == notificationStatus.Name || x.NameAr == notificationStatus.NameAr) && x.NotificationTypeId != notificationStatus.NotificationTypeId).FirstOrDefault();

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
