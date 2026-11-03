using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model;
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

namespace ArabazonPlatform.Application.Services.NotificationManagement.NotificationStatusManagement.Queries
{
    public class NotificationStatusesQuery
    {
        public class Query : IRequest<BaseResponse<List<NotificationStatusModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }


            public Query(int pageNumber, int pageSize, string? searchText = null)
            {
                this.PageNumber = pageNumber;
                this.PageSize = pageSize;
                this.SearchText = searchText;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<NotificationStatusModel>>>
        {
            public async Task<BaseResponse<List<NotificationStatusModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<NotificationStatusModel> notificationStatuses = _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationStatus>().GetByExpressionAsNoTracking(p => (string.IsNullOrEmpty(request.SearchText) || (p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)))).Select(p => new NotificationStatusModel()
                    {
                        NotificationStatusId = p.NotificationStatusId,
                        Name = p.Name ,
                        NameAr = p.NameAr
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<NotificationStatusModel>>(true, string.Empty, string.Empty, notificationStatuses);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationStatusesQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<NotificationStatusModel>>(false, ex.Message);
                }
            }
        }
    }
}
