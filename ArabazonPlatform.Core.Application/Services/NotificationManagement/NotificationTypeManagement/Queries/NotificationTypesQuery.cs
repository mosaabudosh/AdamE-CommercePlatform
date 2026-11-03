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

namespace ArabazonPlatform.Application.Services.NotificationManagement.NotificationTypeManagement.Queries
{
    public class NotificationTypesQuery
    {
        public class Query : IRequest<BaseResponse<List<NotificationTypeModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<NotificationTypeModel>>>
        {
            public async Task<BaseResponse<List<NotificationTypeModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<NotificationTypeModel> notificationTypes = _unitOfWork.GetRepository<Domain.Domains.Notification.NotificationType>().GetByExpressionAsNoTracking(p => (string.IsNullOrEmpty(request.SearchText) || (p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)))).Select(p => new NotificationTypeModel()
                    {
                        NotificationTypeId = p.NotificationTypeId,
                        Name = p.Name,
                        NameAr = p.NameAr,
                        Code = p.Code,
                        IsPlaySound = p.IsPlaySound,
                        Icon = p.Icon,
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<NotificationTypeModel>>(true, string.Empty, string.Empty, notificationTypes);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationTypesQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<NotificationTypeModel>>(false, ex.Message);
                }
            }
        }
    }
}
