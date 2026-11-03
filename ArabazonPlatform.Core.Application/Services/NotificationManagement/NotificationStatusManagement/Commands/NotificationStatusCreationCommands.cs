using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model;
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

namespace ArabazonPlatform.Application.Services.NotificationManagement.NotificationStatusManagement.Commands
{
    public class NotificationStatusCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public NotificationStatusModel NotificationStatusModel { get; set; }
            public Command(NotificationStatusModel notificationStatusModel)
            {
                this.NotificationStatusModel = notificationStatusModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Notification.NotificationStatus notificationStatus = new Domain.Domains.Notification.NotificationStatus()
                    {
                        Name = request.NotificationStatusModel.Name.Trim(),
                        NameAr = request.NotificationStatusModel.NameAr.Trim(),
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(notificationStatus);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationStatus>().Insert(notificationStatus);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationStatusCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Domain.Domains.Notification.NotificationStatus notificationStatus)
            {
                try
                {
                    Domain.Domains.Notification.NotificationStatus existingRecord = _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationStatus>().GetByExpressionAsNoTracking(x => x.Name == notificationStatus.Name || x.NameAr == notificationStatus.NameAr).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), notificationStatus);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
