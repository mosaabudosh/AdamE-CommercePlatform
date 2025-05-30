using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands
{
    public class UserSettingsUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public List<UserSettingModel> UserSettingModels { get; set; }
            public Command(List<UserSettingModel> userSettingModels)
            {
                UserSettingModels = userSettingModels;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    List<int> userSettingIds = request.UserSettingModels.Select(s => s.UserSettingId).ToList();
                    List<Domain.Domains.Authorisation.UserSetting> userSettings = _unitOfWork.GetRepository<Domain.Domains.Authorisation.UserSetting>().GetByExpressionAsNoTracking(p => userSettingIds.Contains(p.UserSettingId)).ToList();

                    userSettings.ForEach(userSetting =>
                    {
                        userSetting.Value = request.UserSettingModels.FirstOrDefault(p => p.UserSettingId == userSetting.UserSettingId).Value;
                        userSetting.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                        userSetting.LastUpdatedDate = DateTime.UtcNow;

                    });

                    _unitOfWork.GetRepository<Domain.Domains.Authorisation.UserSetting>().UpdateRange(userSettings);
                    await _unitOfWork.CommitChanges();

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UserSettingsUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
