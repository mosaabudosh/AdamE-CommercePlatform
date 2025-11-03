using Adam.Logging.Interfaces;
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

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomerCardInfoManagement.Commands
{
    public class CustomerCardInfoDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int CustomerCardInfoId { get; set; }
            public Command(int customerCardInfoId)
            {
                CustomerCardInfoId = customerCardInfoId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    CustomerCardInfo customerCardInfo = _unitOfWork.GetRepository<CustomerCardInfo>().GetByExpression(p => p.CustomerCardInfoId == request.CustomerCardInfoId).SingleOrDefault();

                    _unitOfWork.GetRepository<CustomerCardInfo>().Delete(customerCardInfo);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse<UserModel>(true, string.Empty);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CustomerCardInfoDeletionCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
