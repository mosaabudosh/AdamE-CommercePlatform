using Adam.Logging.Interfaces;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.Authentication;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.Enum;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Commands
{
    public class CustomerLoginCommands
    {
        public class Command : IRequest<BaseResponse<CustomerModel>>
        {
            public string UserName;
            public string Password;
            public Command(string userName, string password)
            {
                UserName = userName;
                Password = password;
            }
            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IAuthenticationService _authenticationService, ITokenHandler _tokenHandler, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse<CustomerModel>>
            {
                public async Task<BaseResponse<CustomerModel>> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        IQueryable<Customer> customers = _unitOfWork.GetRepository<Customer>().GetByExpression(x => x.UserName == request.UserName && x.IsActive == true, includedEntities: new List<System.Linq.Expressions.Expression<Func<Customer, object>>>
                        {
                            z => z.CustomerLogins.Where(p => !string.IsNullOrEmpty(p.AccessToken)),
                            z => z.CustomerDeliveryAddresses,
                            z => z.CustomerCardInfos,
                        });
                        Customer customer = await _unitOfWork.GetRepository<Customer>().FirstOrDefaultAsync(customers);
                        if (customer is null)
                        {
                            return new BaseResponse<CustomerModel>(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                        }
                        if (!_authenticationService.VerifyPasswordHash(request.Password, customer.PasswordHash, customer.PasswordSalt))
                        {
                            return new BaseResponse<CustomerModel>(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                        }
                        if (customer.CustomerLogins.Any())
                        {
                            foreach (CustomerLogin exsistCustomerLogin in customer.CustomerLogins)
                            {
                                await _mediator.Send(new KillCustomerRefreshAccessTokenCommands.Command(exsistCustomerLogin, false));
                            }
                        }
                        customer.CustomerLogins = new List<CustomerLogin>();
                        CustomerLogin customerLogin = new CustomerLogin()
                        {
                            CustomerId = customer.CustomerId,
                            Customer = customer,
                            LoginDate = DateTime.UtcNow,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        };
                        customerLogin = _tokenHandler.BuildCustomerTokens(customerLogin);

                        _unitOfWork.GetRepository<CustomerLogin>().Insert(customerLogin);

                        await _unitOfWork.CommitChanges();

                        CustomerModel customerModel = new CustomerModel()
                        {
                            CustomerId = customer.CustomerId.ToString(),
                            UserName = customer.UserName,
                            FirstName = customer.FirstName,
                            FirstNameAr = customer.FirstNameAr,
                            LastName = customer.LastName,
                            LastNameAr = customer.LastNameAr,
                            AccessToken = customerLogin.AccessToken,
                            RefreshToken = customerLogin.RefreshToken,
                            GenderId = customer.GenderId,
                            ProfileImage = customer.ProfileImage,
                            CityId = customer.CityId,
                            Email = customer.Email,
                            Adresse = customer.Adresse,
                            MobileNumber = customer.MobileNumber,
                            CustomerCardInfoModels = customer.CustomerCardInfos is null ? new List<CustomerCardInfoModel>() : customer.CustomerCardInfos.Select(s => new CustomerCardInfoModel()
                            {
                                CardNumber = s.CardNumber,
                                CustomerCardInfoId = s.CustomerCardInfoId,
                                CustomerId = s.CustomerId,
                                Expiry = s.Expiry,
                                IsRememberCard = s.IsRememberCard,
                            }).ToList(),
                            CustomerDeliveryAddressModels = customer.CustomerDeliveryAddresses is null ? new List<CustomerDeliveryAddressModel>() : customer.CustomerDeliveryAddresses.Select(s => new CustomerDeliveryAddressModel()
                            {
                                Address = s.Address,
                                AddressLabel = s.AddressLabel,
                                CustomerDeliveryAddressId = s.CustomerDeliveryAddressId,
                                CustomerId = s.CustomerId,
                                CustomerName = $"{customer.FirstName} {customer.LastName}",
                                CustomerMobileNumber = customer.MobileNumber,
                                IsDefaultAddress = s.IsDefaultAddress,
                                Latitude = s.Latitude,
                                Longitude = s.Longitude,
                                Name = s.Name
                            }).ToList()
                        };
                        if (!string.IsNullOrEmpty(customerModel.ProfileImage))
                        {
                            string DMSCustomerPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCustomerPath];
                            string folderPath = $"{DMSCustomerPath}\\{customerModel.CustomerId}";
                            BaseResponse<List<Adam.Generic.Generic.FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFolderPathQuery.Query(folderPath));
                            if (!responseGetFileCreationModels.IsSuccess)
                            {
                                return new BaseResponse<CustomerModel>(false, responseGetFileCreationModels.Message);
                            }
                            if (responseGetFileCreationModels.DataObject.Any())
                            {
                                customerModel.FileCreationModel = responseGetFileCreationModels.DataObject.FirstOrDefault();
                            }
                        }
                        return new BaseResponse<CustomerModel>(true, string.Empty, string.Empty, customerModel);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "CustomerLoginCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse<CustomerModel>(false, ex.Message);
                    }
                }
                
            }
        }
    }
}
