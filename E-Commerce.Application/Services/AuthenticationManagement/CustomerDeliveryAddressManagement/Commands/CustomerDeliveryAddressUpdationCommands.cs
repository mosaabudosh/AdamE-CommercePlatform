using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
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
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomerDeliveryAddressManagement.Commands
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
