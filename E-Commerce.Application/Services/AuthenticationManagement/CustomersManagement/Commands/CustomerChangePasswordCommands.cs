using Adam.Logging.Interfaces;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands;
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
using static Adam.Generic.Constants.Constants;

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Commands
{
    public class CustomerChangePasswordCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public Guid CustomerId;
            public string OldPassword;
            public string NewPassword;

            public Command(Guid customerId, string oldPassword, string newPassword)
            {
                CustomerId = customerId;
                OldPassword = oldPassword;
                NewPassword = newPassword;
            }
            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IAuthenticationService _authenticationService, IMediator _mediator) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        Customer customer = _unitOfWork.GetRepository<Customer>().GetByExpressionAsNoTracking(x => x.CustomerId == request.CustomerId, includedEntities: new List<System.Linq.Expressions.Expression<Func<Customer, object>>> { z => z.CustomerLogins.Where(p => !p.LogOutDate.HasValue) }).SingleOrDefault();
                        if (customer is null)
                        {
                            return new BaseResponse(false, string.Empty, ErrorCode.InvalidUser);
                        }
                        if (string.IsNullOrEmpty(request.OldPassword))
                        {
                            return new BaseResponse(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                        }
                        if (!_authenticationService.VerifyPasswordHash(request.OldPassword, customer.PasswordHash, customer.PasswordSalt))
                        {
                            return new BaseResponse(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                        }
                        byte[] passwordHash, passwordSalt;
                        _authenticationService.HashPassword(request.NewPassword, out passwordHash, out passwordSalt);
                        customer.PasswordHash = passwordHash;
                        customer.PasswordSalt = passwordSalt;
                        _unitOfWork.GetRepository<Customer>().Update(customer);
                        foreach (CustomerLogin exsistCustomerLogin in customer.CustomerLogins)
                        {
                            await _mediator.Send(new KillCustomerRefreshAccessTokenCommands.Command(exsistCustomerLogin, false));
                        }
                        await _unitOfWork.CommitChanges();
                        return new BaseResponse(true);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "CustomerChangePasswordCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse(false, ex.Message);
                    }
                }

            }
        }
    }
}
