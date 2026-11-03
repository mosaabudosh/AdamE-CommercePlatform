using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
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
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.CustomerDeliveryAddressManagement.Commands
{
    public class CustomerDeliveryAddressUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CustomerDeliveryAddressModel CustomerDeliveryAddressModel { get; set; }
            public Command(CustomerDeliveryAddressModel customerDeliveryAddressModel)
            {
                CustomerDeliveryAddressModel = customerDeliveryAddressModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _customerAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                try
                {
                    CustomerDeliveryAddress customerDeliveryAddress = _unitOfWork.GetRepository<CustomerDeliveryAddress>().GetByExpressionAsNoTracking(x => x.CustomerDeliveryAddressId == request.CustomerDeliveryAddressModel.CustomerDeliveryAddressId).SingleOrDefault();


                    customerDeliveryAddress.IsDefaultAddress = request.CustomerDeliveryAddressModel.IsDefaultAddress;
                    customerDeliveryAddress.Address = request.CustomerDeliveryAddressModel.Address;
                    customerDeliveryAddress.AddressLabel = request.CustomerDeliveryAddressModel.AddressLabel;
                    customerDeliveryAddress.Latitude = request.CustomerDeliveryAddressModel.Latitude;
                    customerDeliveryAddress.Longitude = request.CustomerDeliveryAddressModel.Longitude;
                    customerDeliveryAddress.Name = request.CustomerDeliveryAddressModel.Name;

                    customerDeliveryAddress.LastUpdatedDate = DateTime.UtcNow;
                    customerDeliveryAddress.LastUpdateUserId = _customerAccessor.GetCurrentUserId();

                    _unitOfWork.GetRepository<CustomerDeliveryAddress>().Update(customerDeliveryAddress);
                    await _unitOfWork.CommitChanges();
                                     
                    return new BaseResponse(true, string.Empty);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CustomerDeliveryAddressUpdationCommands", _customerAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
