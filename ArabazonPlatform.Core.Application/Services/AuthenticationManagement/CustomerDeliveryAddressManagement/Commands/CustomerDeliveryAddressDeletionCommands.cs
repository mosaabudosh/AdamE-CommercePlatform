using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Domain.Domains.Authentication;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.CustomerDeliveryAddressManagement.Commands
{
    public class CustomerDeliveryAddressDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int CustomerDeliveryAddressId { get; set; }
            public Command(int customerDeliveryAddressId)
            {
                CustomerDeliveryAddressId = customerDeliveryAddressId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    CustomerDeliveryAddress customerDeliveryAddress = _unitOfWork.GetRepository<CustomerDeliveryAddress>().GetByExpression(p => p.CustomerDeliveryAddressId == request.CustomerDeliveryAddressId).SingleOrDefault();

                    _unitOfWork.GetRepository<CustomerDeliveryAddress>().Delete(customerDeliveryAddress);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse<UserModel>(true, string.Empty);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CustomerDeliveryAddressDeletionCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
