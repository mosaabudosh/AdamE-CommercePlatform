using Adam.Logging.Interfaces;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
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
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Commands
{
    public class ReRegistrationCustomerCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CustomerRegisterModel CustomerRegisterModel { get; set; }
            public Command(CustomerRegisterModel customerRegisterModel)
            {
                CustomerRegisterModel = customerRegisterModel;
            }

            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, IAuthenticationService _authenticationService) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        IQueryable<Customer> customersQueryable = _unitOfWork.GetRepository<Customer>().GetByExpression(x => x.CustomerId == request.CustomerRegisterModel.CustomerId);
                        Customer exsistCustomer = await _unitOfWork.GetRepository<Customer>().FirstOrDefaultAsync(customersQueryable);
                  
                        if (!string.IsNullOrEmpty(request.CustomerRegisterModel.Password))
                        {
                            byte[] passwordHash, passwordSalt;
                            _authenticationService.HashPassword(request.CustomerRegisterModel.Password, out passwordHash, out passwordSalt);
                            exsistCustomer.PasswordHash = passwordHash;
                            exsistCustomer.PasswordSalt = passwordSalt;
                            exsistCustomer.OTPCode = null;
                        }
                        else
                        {
                            exsistCustomer.OTPCode = await _mediator.Send(new GenerateOTPCodeCommands.Command());
                        }

                        await _unitOfWork.CommitChanges();

                        return new BaseResponse(true, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "ReRegistrationCustomerCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse(false, ex.Message);
                    }
                }

            }
        }
    }
}
