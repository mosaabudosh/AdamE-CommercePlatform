using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.NotificationManagement.Notification.Queries
{
    public class NotificationsQueryByUserId
    {
        public class Query : IRequest<BaseResponse<List<NotificationModel>>>
        {
            public Guid UserId { get; set; }
            public Query(Guid userId)
            {
                UserId = userId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<NotificationModel>>>
        {
            public async Task<BaseResponse<List<NotificationModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<NotificationModel> notifications = _unitOfWork.GetRepository<Domain.Domains.Notification.Notification>().GetByExpression(p => p.UserId == request.UserId).Select(s => new NotificationModel()
                    {
                        NotificationId = s.NotificationId,
                        UserId = s.UserId,
                        Description = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Description : s.DescriptionAr,
                        NotificationCode = s.NotificationCode,
                        NotificationStatus = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.NotificationStatus.Name : s.NotificationStatus.NameAr,
                        NotificationType = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.NotificationType.Name : s.NotificationType.NameAr,
                        Subject = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Subject : s.SubjectAr,
                        NotificationStatusId = s.NotificationStatusId,
                        NotificationTypeId = s.NotificationTypeId,
                        Tooltip = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Tooltip : s.TooltipAr,
                        CreationDate = s.CreationDate,
                        DescriptionAr = s.DescriptionAr,
                        SubjectAr = s.SubjectAr,
                        TooltipAr = s.TooltipAr,
                        Customer = s.CustomerId.HasValue ? (_userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? $"{s.Customer.FirstName} {s.Customer.LastName}" : $"{s.Customer.FirstNameAr} {s.Customer.LastNameAr}") : string.Empty
                    }).OrderByDescending(p => p.NotificationStatusId).ToList();
                    return new BaseResponse<List<NotificationModel>>(true, string.Empty, string.Empty, notifications);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "NotificationsQueryByUserId", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<NotificationModel>>(false, ex.Message);
                }
            }
        }
    }
}
