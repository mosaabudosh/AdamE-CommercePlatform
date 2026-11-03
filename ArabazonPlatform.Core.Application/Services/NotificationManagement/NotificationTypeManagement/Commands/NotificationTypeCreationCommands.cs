using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.Notification;
using ArabazonPlatform.Domain.Domains.Notification;
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
    public class NotificationTypeCreationCommands
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
                    Domain.Domains.Notification.NotificationType notificationType = new Domain.Domains.Notification.NotificationType()
                    {
                        Name = request.NotificationTypeModel.Name.Trim(),
                        NameAr = request.NotificationTypeModel.NameAr.Trim(),
                        Icon = request.NotificationTypeModel.Icon,
                        IsPlaySound = request.NotificationTypeModel.IsPlaySound,
                        Code = request.NotificationTypeModel.Code,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(notificationType);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationType>().Insert(notificationType);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationTypeCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Domain.Domains.Notification.NotificationType notificationType)
            {
                try
                {
                    Domain.Domains.Notification.NotificationType existingRecord = _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationType>().GetByExpressionAsNoTracking(x => x.Name == notificationType.Name || x.NameAr == notificationType.NameAr).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), notificationType);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
