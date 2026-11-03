using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Services.AuthenticationManagement.UsersManagement.Commands;
using ArabazonPlatform.Domain.Domains.Authentication;
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
using static ArabazonPlatform.Generic.Constants.Constants;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.CustomersManagement.Commands
{
    public class CustomerLogOutCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public string RefreshToken;

            public Command(string refreshToken)
            {
                RefreshToken = refreshToken;
            }
            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        IQueryable<CustomerLogin> customerLogins = _unitOfWork.GetRepository<CustomerLogin>().GetByExpressionAsNoTracking(x => x.RefreshToken == request.RefreshToken);
                        CustomerLogin customerLogin = await _unitOfWork.GetRepository<CustomerLogin>().FirstOrDefaultAsync(customerLogins);
                        if (customerLogin is null)
                        {
                            return new BaseResponse<string>(false, string.Empty, ErrorCode.InvalidRefreshToken, string.Empty);
                        }
                        BaseResponse killRefreshAccessTokenResponse = await _mediator.Send(new KillCustomerRefreshAccessTokenCommands.Command(customerLogin));
                        if (!killRefreshAccessTokenResponse.IsSuccess)
                        {
                            return new BaseResponse<string>(false, killRefreshAccessTokenResponse.Message, killRefreshAccessTokenResponse.ErrorCode);
                        }
                        return new BaseResponse(true);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "CustomerLogOutCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse(false, ex.Message);
                    }
                }
            }
        }
    }
}
