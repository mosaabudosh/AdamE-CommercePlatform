using Adam.Logging.Interfaces;
using E_Commerce.Domain.Domains.Authentication;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Commands
{
    public class KillCustomerRefreshAccessTokenCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CustomerLogin CustomerLogin;
            public bool IsCommit;
            public Command(CustomerLogin customerLogin, bool isCommit = true)
            {
                CustomerLogin = customerLogin;
                IsCommit = isCommit;
            }
            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        request.CustomerLogin.LogOutDate = DateTime.Now;
                        request.CustomerLogin.RefreshTokenExpirationDate = 0;
                        request.CustomerLogin.RefreshToken = string.Empty;
                        request.CustomerLogin.AccessToken = string.Empty;
                        request.CustomerLogin.IsActive = false;
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
                        await _exceptionService.LogErrorAsync(ex, "KillCustomerRefreshAccessTokenCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse(false, ex.Message);
                    }
                }

            }
        }
    }
}
