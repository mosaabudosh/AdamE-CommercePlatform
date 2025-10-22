using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Authorisation;
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
    public class CustomerDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public Guid CustomerId { get; set; }
            public Command(Guid customerId)
            {
                CustomerId = customerId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Customer customer = _unitOfWork.GetRepository<Customer>().GetByExpression(p => p.CustomerId == request.CustomerId).SingleOrDefault();
                    customer.IsActive = !customer.IsActive;

                    customer.DeletedDate = DateTime.UtcNow;
                    customer.DeletedUserId = _userAccessor.GetCurrentUserId();
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse<UserModel>(true, string.Empty);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CustomerDeletionCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
