using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Authentication.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains.Authorisation;
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

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.UsersManagement.Commands
{
    public class KillUserRefreshAccessTokenCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public UserLogin UserLogin;
            public bool IsCommit;
            public Command(UserLogin userLogin, bool isCommit = true)
            {
                UserLogin = userLogin;
                IsCommit = isCommit;
            }
            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        request.UserLogin.LogOutDate = DateTime.Now;
                        request.UserLogin.RefreshTokenExpirationDate = 0;
                        request.UserLogin.RefreshToken = string.Empty;
                        request.UserLogin.AccessToken = string.Empty;
                        request.UserLogin.IsActive = false;
                        if (request.IsCommit)
                        {
                            await _unitOfWork.CommitChanges();
                        }
                        return new BaseResponse(true);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "KillUserRefreshAccessTokenCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse(false, ex.Message);
                    }
                }

            }
        }
    }
}
