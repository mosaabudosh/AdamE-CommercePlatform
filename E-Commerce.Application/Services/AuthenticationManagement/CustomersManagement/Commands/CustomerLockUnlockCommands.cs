using Adam.Logging.Interfaces;
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
    public class CustomerLockUnlockCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public Guid CustomerId;
            public bool isLock;

            public Command(Guid customerId, bool isLock)
            {
                this.CustomerId = customerId;
                this.isLock = isLock;
            }

            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        Customer customer = _unitOfWork.GetRepository<Customer>().GetByExpressionAsNoTracking(p => p.CustomerId == request.CustomerId).FirstOrDefault();
                        customer.IsBlock = request.isLock;

                        await _unitOfWork.CommitChanges();

                        return new BaseResponse(true, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "CustomerLockUnlockCommands", _userAccessor.GetCurrentUserId());
                        return new BaseResponse(false, ex.Message);
                    }
                }
            }

        }
    }
}
