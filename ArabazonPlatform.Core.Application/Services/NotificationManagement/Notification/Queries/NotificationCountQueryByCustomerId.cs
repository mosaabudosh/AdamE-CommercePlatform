using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains.Authorisation;
using ArabazonPlatform.Domain.Enum;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.NotificationManagement.Notification.Queries
{
    public class NotificationCountQueryByCustomerId
    {
        public class Query : IRequest<BaseResponse<int>>
        {
            public Guid CustomerId { get; set; }
            public Query(Guid customerId)
            {
                CustomerId = customerId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<int>>
        {
            public async Task<BaseResponse<int>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    int notificationCount = _unitOfWork.GetRepository<Domain.Domains.Notification.Notification>().GetByExpression(p => p.CustomerId == request.CustomerId && p.NotificationStatusId == (int)Enums.NotificationStatuses.UnRead).Count();
                    return new BaseResponse<int>(true, string.Empty, string.Empty, notificationCount);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationCountQueryByCustomerId", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<int>(false, ex.Message);
                }
            }
        }
    }
}
