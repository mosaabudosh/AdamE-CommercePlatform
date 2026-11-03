using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.NotificationManagement.Notification.Commands
{
    public class NotificationCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public List<NotificationModel> NotificationModels { get; set; }
            public Command(List<NotificationModel> notificationModels)
            {
                NotificationModels = notificationModels;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    List<Domain.Domains.Notification.Notification> notifications = request.NotificationModels.Select(s => new Domain.Domains.Notification.Notification()
                    {
                        Subject = s.Subject,
                        SubjectAr = s.SubjectAr,
                        Description = s.Description,
                        DescriptionAr = s.DescriptionAr,
                        NotificationCode = s.NotificationCode,
                        NotificationStatusId = s.NotificationStatusId,
                        NotificationTypeId = s.NotificationTypeId,
                        Tooltip = s.Tooltip,
                        TooltipAr = s.TooltipAr,
                        UserId = s.UserId,
                        CustomerId = s.CustomerId,                        
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    }).ToList();
                  
                    _unitOfWork.GetRepository<Domain.Domains.Notification.Notification>().AddRange(notifications);
                    
                    await _unitOfWork.CommitChanges();

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
            
        }
    }
}
