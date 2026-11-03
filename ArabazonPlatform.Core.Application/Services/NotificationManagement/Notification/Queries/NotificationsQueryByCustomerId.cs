using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.NotificationManagement.Notification.Queries
{
    public class NotificationsQueryByCustomerId
    {
        public class Query : IRequest<BaseResponse<List<NotificationModel>>>
        {
            public Guid CustomerId { get; set; }
            public Query(Guid customerId)
            {
                CustomerId = customerId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<NotificationModel>>>
        {
            public async Task<BaseResponse<List<NotificationModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<NotificationModel> notifications = _unitOfWork.GetRepository<Domain.Domains.Notification.Notification>().GetByExpression(p => p.CustomerId == request.CustomerId).Select(s => new NotificationModel()
                    {
                        NotificationId = s.NotificationId,
                        CustomerId = s.CustomerId,
                        Description = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Description : s.DescriptionAr,
                        NotificationCode = s.NotificationCode,
                        NotificationStatus = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.NotificationStatus.Name : s.NotificationStatus.NameAr,
                        NotificationType = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.NotificationType.Name : s.NotificationType.NameAr,
                        Subject = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Subject : s.SubjectAr,
                        NotificationStatusId = s.NotificationStatusId,
                        NotificationTypeId = s.NotificationTypeId,
                        Tooltip = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Tooltip : s.TooltipAr,
                        CreationDate = s.CreationDate,
                        DescriptionAr = s.DescriptionAr,
                        SubjectAr = s.SubjectAr,
                        TooltipAr = s.TooltipAr,
                    }).OrderByDescending(p => p.NotificationStatusId).ToList();
                    return new BaseResponse<List<NotificationModel>>(true, string.Empty, string.Empty, notifications);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationsQueryByCustomerId", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<NotificationModel>>(false, ex.Message);
                }
            }
        }
    }
}
