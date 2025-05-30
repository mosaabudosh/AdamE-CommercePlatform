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

namespace E_Commerce.Application.Services.NotificationManagement.NotificationStatusManagement.Queries
{
    public class NotificationStatusQueryById
    {
        public class Query : IRequest<BaseResponse<NotificationStatusModel>>
        {
            public int NotificationStatusId { get; set; }
            public Query(int notificationStatusId)
            {
                this.NotificationStatusId = notificationStatusId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<NotificationStatusModel>>
        {
            public async Task<BaseResponse<NotificationStatusModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    NotificationStatusModel notificationStatus = _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationStatus>().GetByExpressionAsNoTracking(p => p.NotificationStatusId == request.NotificationStatusId).Select(p => new NotificationStatusModel()
                    {
                        NotificationStatusId = p.NotificationStatusId,
                        Name = p.Name ,
                        NameAr = p.NameAr
                    }).FirstOrDefault();
                    return new BaseResponse<NotificationStatusModel>(true, string.Empty, string.Empty, notificationStatus);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationStatusQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<NotificationStatusModel>(false, ex.Message);
                }
            }
        }
    }
}
