using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.Authorisation;
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
    public class VerifyCustomerVerificationCodeCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CustomerRegisterModel CustomerRegisterModel { get; set; }
            public Command(CustomerRegisterModel customerRegisterModel)
            {
                CustomerRegisterModel = customerRegisterModel;
            }

            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        IQueryable<Customer> customersQueryable = _unitOfWork.GetRepository<Customer>().GetByExpression(x => x.CustomerId == request.CustomerRegisterModel.CustomerId);
                        Customer exsistCustomer = await _unitOfWork.GetRepository<Customer>().FirstOrDefaultAsync(customersQueryable);

                        if (exsistCustomer is not null && !string.IsNullOrEmpty(exsistCustomer.OTPCode) && !string.IsNullOrEmpty(request.CustomerRegisterModel.OTPCode) && exsistCustomer.OTPCode == request.CustomerRegisterModel.OTPCode) 
                        {
                            return new BaseResponse<CustomerRegisterModel>(true, string.Empty);
                        }

                        return new BaseResponse<CustomerRegisterModel>(false, string.Empty, ErrorCode.Invalid);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "VerifyCustomerVerificationCodeCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse<CustomerRegisterModel>(false, ex.Message);
                    }
                }

            }
        }
    }
}
