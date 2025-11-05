using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Domain.Domains.Authentication;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomerDeliveryAddressManagement.Commands
{
    public class CustomerDeliveryAddressCreateCommands
    {
        public class Command : IRequest<BaseResponse<int>>
        {
            public CustomerDeliveryAddressModel CustomerDeliveryAddressModel { get; set; }
            public Command(CustomerDeliveryAddressModel customerDeliveryAddressModel)
            {
                CustomerDeliveryAddressModel = customerDeliveryAddressModel;
            }

            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse<int>>
            {
                public async Task<BaseResponse<int>> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        CustomerDeliveryAddress customerDeliveryAddress = new CustomerDeliveryAddress()
                        {
                            Address = request.CustomerDeliveryAddressModel.Address,
                            AddressLabel = request.CustomerDeliveryAddressModel.AddressLabel,
                            Latitude = request.CustomerDeliveryAddressModel.Latitude,
                            Longitude = request.CustomerDeliveryAddressModel.Longitude,
                            Name = request.CustomerDeliveryAddressModel.Name,                            
                            CustomerId = request.CustomerDeliveryAddressModel.CustomerId,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        };

                        _unitOfWork.GetRepository<CustomerDeliveryAddress>().Insert(customerDeliveryAddress);
                        await _unitOfWork.CommitChanges();

                        return new BaseResponse<int>(true, string.Empty,customerDeliveryAddress.CustomerDeliveryAddressId);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "CustomerDeliveryAddressCreateCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse<int>(false, ex.Message);
                    }
                }

            }
        }
    }
}
