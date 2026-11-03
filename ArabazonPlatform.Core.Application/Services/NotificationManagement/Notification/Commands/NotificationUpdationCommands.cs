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

namespace ArabazonPlatform.Application.Services.NotificationManagement.Notification.Commands
{
    public class NotificationUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public List<NotificationUpdationModel> NotificationUpdationModels { get; set; }
            public Command(List<NotificationUpdationModel> notificationUpdationModels)
            {
                this.NotificationUpdationModels = notificationUpdationModels;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    List<Guid> notificationIds = request.NotificationUpdationModels.Select(s => s.NotificationId).ToList();
                    List<Domain.Domains.Notification.Notification> notifications = _unitOfWork.GetRepository<Domain.Domains.Notification.Notification>().GetByExpression(p => notificationIds.Contains(p.NotificationId)).ToList();

                    int notificationStatusId = request.NotificationUpdationModels.FirstOrDefault().NotificationStatusId;

                    notifications.Select(n =>
                    {
                        n.NotificationStatusId = notificationStatusId;
                        n.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                        n.LastUpdatedDate = DateTime.UtcNow;
                        return n;
                    }).ToList();

                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
