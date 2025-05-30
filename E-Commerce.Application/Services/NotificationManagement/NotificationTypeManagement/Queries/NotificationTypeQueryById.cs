using Adam.Logging.Interfaces;
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

namespace E_Commerce.Application.Services.NotificationManagement.NotificationTypeManagement.Queries
{
    public class NotificationTypeQueryById
    {
        public class Query : IRequest<BaseResponse<NotificationTypeModel>>
        {
            public int NotificationTypeId { get; set; }
            public Query(int notificationTypeId)
            {
                this.NotificationTypeId = notificationTypeId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<NotificationTypeModel>>
        {
            public async Task<BaseResponse<NotificationTypeModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    NotificationTypeModel notificationType = _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationType>().GetByExpressionAsNoTracking(p => p.NotificationTypeId == request.NotificationTypeId).Select(p => new NotificationTypeModel()
                    {
                        NotificationTypeId = p.NotificationTypeId,
                        Name = p.Name,
                        NameAr = p.NameAr,
                        Code = p.Code,
                        Icon = p.Icon,
                        IsPlaySound = p.IsPlaySound,
                    }).FirstOrDefault();
                    return new BaseResponse<NotificationTypeModel>(true, string.Empty, string.Empty, notificationType);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationTypeQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<NotificationTypeModel>(false, ex.Message);
                }
            }
        }
    }
}
