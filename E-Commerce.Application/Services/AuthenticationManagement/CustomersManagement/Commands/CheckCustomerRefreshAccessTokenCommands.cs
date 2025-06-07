using Adam.Logging.Interfaces;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Model.Authorisation;
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
    public class CheckCustomerRefreshAccessTokenCommands
    {
        public class Command : IRequest<BaseResponse<CustomerModel>>
        {
            public string RefreshToken;

            public Command(string refreshToken)
            {
                RefreshToken = refreshToken;
            }
            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, ITokenHandler _tokenHandler) : IRequestHandler<Command, BaseResponse<CustomerModel>>
            {
                public async Task<BaseResponse<CustomerModel>> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        IQueryable<CustomerLogin> customerLogins = _unitOfWork.GetRepository<CustomerLogin>().GetByExpression(x => x.RefreshToken == request.RefreshToken && x.IsActive, includedEntities: new List<System.Linq.Expressions.Expression<Func<CustomerLogin, object>>>
                        {
                            z => z.Customer
                        });
                        CustomerLogin customerLogin = await _unitOfWork.GetRepository<CustomerLogin>().FirstOrDefaultAsync(customerLogins);
                        if (customerLogin is null)
                        {
                            return new BaseResponse<CustomerModel>(false, string.Empty, ErrorCode.InvalidRefreshToken);
                        }
                        //if (DateTime.Now.Ticks > customerLogin.RefreshTokenExpirationDate)
                        //{
                        //    BaseResponse killRefreshAccessTokenResponse = await KillRefreshAccessToken(customerLogin);
                        //    if (!killRefreshAccessTokenResponse.IsSuccess)
                        //    {
                        //        return new BaseResponse<CustomerModel>(false, killRefreshAccessTokenResponse.Message, killRefreshAccessTokenResponse.ErrorCode);
                        //    }
                        //    return new BaseResponse<CustomerModel>(false, string.Empty, ErrorCode.InvalidRefreshToken);
                        //}
                        customerLogin = _tokenHandler.BuildCustomerTokens(customerLogin);
                        await _unitOfWork.CommitChanges();
                        CustomerModel customerModel = new CustomerModel()
                        {
                            CustomerId = customerLogin.CustomerId.ToString(),
                            UserName = customerLogin.Customer.UserName,
                            FirstName = customerLogin.Customer.FirstName,
                            FirstNameAr = customerLogin.Customer.FirstNameAr,
                            LastName = customerLogin.Customer.LastName,
                            LastNameAr = customerLogin.Customer.LastNameAr,
                            AccessToken = customerLogin.AccessToken,
                            RefreshToken = customerLogin.RefreshToken
                        };
                        return new BaseResponse<CustomerModel>(true, string.Empty, string.Empty, customerModel);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "CheckCustomerRefreshAccessTokenCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse<CustomerModel>(false, ex.Message);
                    }
                }

            }
        }
    }
}
