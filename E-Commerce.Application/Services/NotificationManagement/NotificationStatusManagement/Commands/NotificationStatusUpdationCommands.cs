using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Application.Model.Notification;
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
    public class NotificationStatusUpdationCommands
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
                        NotificationStatusId = request.NotificationStatusModel.NotificationStatusId,
                        Name = request.NotificationStatusModel.Name.Trim(),
                        NameAr = request.NotificationStatusModel.NameAr.Trim(),
                        LastUpdateUserId = _userAccessor.GetCurrentUserId(),
                        LastUpdatedDate = DateTime.UtcNow,
                    };

                    BaseResponse validationBaseResponse = ValidateUpdation(notificationStatus);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationStatus>().Update(notificationStatus);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationStatusUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Domain.Domains.Notification.NotificationStatus notificationStatus)
            {
                try
                {
                    Domain.Domains.Notification.NotificationStatus existingRecord = _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationStatus>().GetByExpressionAsNoTracking(x => (x.Name == notificationStatus.Name || x.NameAr == notificationStatus.NameAr) && x.NotificationStatusId != notificationStatus.NotificationStatusId).FirstOrDefault();

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
