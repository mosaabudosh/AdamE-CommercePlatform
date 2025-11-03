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

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomerDeliveryAddressManagement.Queries
{
    public class CustomerDeliveryAddressByCustomerIdQuery
    {
        public class Query : IRequest<BaseResponse<List<CustomerDeliveryAddressModel>>>
        {
            public Guid CustomerId { get; set; }
            public Query(Guid customerId)
            {
                CustomerId = customerId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<CustomerDeliveryAddressModel>>>
        {
            public async Task<BaseResponse<List<CustomerDeliveryAddressModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CustomerDeliveryAddressModel> customerDeliveryAddressModels = _unitOfWork.GetRepository<CustomerDeliveryAddress>().GetByExpression(p => p.CustomerId == request.CustomerId).Select(s => new CustomerDeliveryAddressModel
                    {
                        CustomerDeliveryAddressId = s.CustomerDeliveryAddressId,
                        Name = s.Name,
                        Longitude = s.Longitude,
                        Latitude = s.Latitude,
                        AddressLabel = s.AddressLabel,
                        Address = s.Address,
                        IsDefaultAddress = s.IsDefaultAddress,
                        CustomerId = s.CustomerId,
                        CustomerMobileNumber = s.Customer.MobileNumber,
                        CustomerName = $"{s.Customer.FirstName} {s.Customer.LastName}"
                    }).ToList();

                    return new BaseResponse<List<CustomerDeliveryAddressModel>>(true, string.Empty, string.Empty, customerDeliveryAddressModels);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CustomerDeliveryAddressByCustomerIdQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<CustomerDeliveryAddressModel>>(false, ex.Message);
                }
            }
        }
    }
}
