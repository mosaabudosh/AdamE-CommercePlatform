using Adam.Generic.Generic;
using E_Commerce.API.Model;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.AuthenticationManagement.CustomerCardInfoManagement.Commands;
using E_Commerce.Application.Services.AuthenticationManagement.CustomerCardInfoManagement.Queries;
using E_Commerce.Application.Services.AuthenticationManagement.CustomerDeliveryAddressManagement.Commands;
using E_Commerce.Application.Services.AuthenticationManagement.CustomerDeliveryAddressManagement.Queries;
using E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Commands;
using E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Queries;
using E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands;
using E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.CountriesManagement.Queries;
using E_Commerce.Domain.Domains.Authentication;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerManagementController(IMediator _mediator) : BaseController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("registration")]
        public async Task<ActionResult<BaseResponse>> Registration([FromBody] CustomerModel customerModel)
        {
            try
            {
                return await _mediator.Send(new CustomerRegistrationCommands.Command(customerModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(Registration), UserAccessor.GetCurrentUserId(), customerModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("re-registration")]
        public async Task<ActionResult<BaseResponse>> ReRegistration([FromBody] CustomerRegisterModel customerRegisterModel)
        {
            try
            {
                return await _mediator.Send(new ReRegistrationCustomerCommands.Command(customerRegisterModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ReRegistration), UserAccessor.GetCurrentUserId(), customerRegisterModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("verify-customer")]
        public async Task<ActionResult<BaseResponse>> VerifyCustomer([FromBody] CustomerRegisterModel customerRegisterModel)
        {
            try
            {
                return await _mediator.Send(new VerifyCustomerVerificationCodeCommands.Command(customerRegisterModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(VerifyCustomer), UserAccessor.GetCurrentUserId(), customerRegisterModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<BaseResponse<CustomerModel>>> Login(LoginModel loginModel)
        {
            try
            {
                return await _mediator.Send(new CustomerLoginCommands.Command(loginModel.UserName, loginModel.Password));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(Login), UserAccessor.GetCurrentUserId(), loginModel);
                return new BaseResponse<CustomerModel>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("logout")]
        public async Task<ActionResult<BaseResponse>> LogOut([FromBody] RefreshTokenModel refreshTokenModel)
        {
            try
            {
                return await _mediator.Send(new CustomerLogOutCommands.Command(refreshTokenModel.RefreshToken));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(LogOut), UserAccessor.GetCurrentUserId(), refreshTokenModel);
                return new BaseResponse<string>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("check-refresh-token")]
        public async Task<BaseResponse<CustomerModel>> CheckRefreshAccessToken([FromBody] RefreshTokenModel refreshTokenModel)
        {
            try
            {
                return await _mediator.Send(new CheckCustomerRefreshAccessTokenCommands.Command(refreshTokenModel.RefreshToken));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CheckRefreshAccessToken), UserAccessor.GetCurrentUserId(), refreshTokenModel);
                return new BaseResponse<CustomerModel>(false, ex.Message);
            }
        }



        #region customer
        [HttpPost]
        [Route("change-password")]
        public async Task<ActionResult<BaseResponse>> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            try
            {
                return await _mediator.Send(new CustomerChangePasswordCommands.Command(changePasswordModel.UserId, changePasswordModel.OldPassword, changePasswordModel.NewPassword));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ChangePassword), UserAccessor.GetCurrentUserId(), changePasswordModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("customers")]
        public async Task<ActionResult<BaseResponse<List<CustomerModel>>>> GetCustomers(int pageNumber, int pageSize, string? searchText, int? cityId,int? languageId)
        {
            try
            {
                return await _mediator.Send(new CustomersQuery.Query(pageNumber, pageSize, searchText, cityId, true));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCustomers), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText, cityId, languageId);
                return new BaseResponse<List<CustomerModel>>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("customers-count")]
        public async Task<ActionResult<BaseResponse<int>>> GetCustomersCount(string? searchText, int? cityId)
        {
            try
            {
                BaseResponse<List<CustomerModel>> responseCustomerModels = await _mediator.Send(new CustomersQuery.Query(0, int.MaxValue, searchText, cityId));
                if (!responseCustomerModels.IsSuccess)
                {
                    return new BaseResponse<int>(false, responseCustomerModels.Message, responseCustomerModels.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, string.Empty, responseCustomerModels.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCustomersCount), UserAccessor.GetCurrentUserId(), searchText, cityId);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("customer-by-id/{customerId}")]
        public async Task<ActionResult<BaseResponse<CustomerModel>>> GetCustomerById(Guid customerId)
        {
            try
            {
                return await _mediator.Send(new GetCustomerByIdQuery.Query(customerId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCustomerById), UserAccessor.GetCurrentUserId(), customerId);
                return new BaseResponse<CustomerModel>(false, ex.Message);
            }
        }

        [HttpPut]
        [Route("customer-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateCustomer([FromBody] CustomerModel customerModel)
        {
            try
            {
                return await _mediator.Send(new CustomerUpdationCommands.Command(customerModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateCustomer), UserAccessor.GetCurrentUserId(), customerModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("customer-deletion/{customerId}")]
        public async Task<ActionResult<BaseResponse>> DeleteCustomer(string customerId)
        {
            try
            {
                return await _mediator.Send(new CustomerDeletionCommands.Command(new Guid(customerId)));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteCustomer), UserAccessor.GetCurrentUserId(), customerId);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("customer-lock-un-lock")]
        public async Task<ActionResult<BaseResponse>> UpdateCustomerLockUnlock([FromBody] CustomerLockUnlockModel customerLockUnlockModel)
        {
            try
            {
                return await _mediator.Send(new CustomerLockUnlockCommands.Command(new Guid(customerLockUnlockModel.CustomerId), customerLockUnlockModel.IsLock));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateCustomerLockUnlock), UserAccessor.GetCurrentUserId(), customerLockUnlockModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("customer-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetCustomersDDL()
        {
            try
            {
                return await _mediator.Send(new CustomersDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCustomersDDL), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }
        #endregion



        #region customer-card-info
        [HttpPost]
        [Route("create-customer-card-info")]
        public async Task<ActionResult<BaseResponse>> CreateCustomerCardInfo([FromBody] CustomerCardInfoModel customerCardInfoModel)
        {
            try
            {
                return await _mediator.Send(new CustomerCardInfoCreateCommands.Command(customerCardInfoModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateCustomerCardInfo), UserAccessor.GetCurrentUserId(), customerCardInfoModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("customer-card-infos-by-customerId/{customerId}")]
        public async Task<ActionResult<BaseResponse<List<CustomerCardInfoModel>>>> GetCustomerCardInfosByCustomerId(Guid customerId)
        {
            try
            {
                return await _mediator.Send(new CustomerCardInfoByCustomerIdQuery.Query(customerId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCustomerCardInfosByCustomerId), UserAccessor.GetCurrentUserId(), customerId);
                return new BaseResponse<List<CustomerCardInfoModel>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("customer-card-info-deletion/{customerCardInfoId}")]
        public async Task<ActionResult<BaseResponse>> DeleteCustomerCardInfo(int customerCardInfoId)
        {
            try
            {
                return await _mediator.Send(new CustomerCardInfoDeletionCommands.Command(customerCardInfoId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteCustomerCardInfo), UserAccessor.GetCurrentUserId(), customerCardInfoId);
                return new BaseResponse(false, ex.Message);
            }
        }
        #endregion


        #region customer-delivery-address
        [HttpPost]
        [Route("create-customer-delivery-address")]
        public async Task<ActionResult<BaseResponse<int>>> CreateCustomerDeliveryAddress([FromBody] CustomerDeliveryAddressModel customerDeliveryAddressModel)
        {
            try
            {
                return await _mediator.Send(new CustomerDeliveryAddressCreateCommands.Command(customerDeliveryAddressModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateCustomerDeliveryAddress), UserAccessor.GetCurrentUserId(), customerDeliveryAddressModel);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [HttpPut]
        [Route("customer-delivery-address-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateCustomerDeliveryAddress([FromBody] CustomerDeliveryAddressModel customerDeliveryAddressModel)
        {
            try
            {
                return await _mediator.Send(new CustomerDeliveryAddressUpdationCommands.Command(customerDeliveryAddressModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateCustomerDeliveryAddress), UserAccessor.GetCurrentUserId(), customerDeliveryAddressModel);
                return new BaseResponse(false, ex.Message);
            }
        }


        [HttpGet]
        [Route("customer-delivery-addresses-by-customerId/{customerId}")]
        public async Task<ActionResult<BaseResponse<List<CustomerDeliveryAddressModel>>>> GetCustomerDeliveryAddressesByCustomerId(Guid customerId)
        {
            try
            {
                return await _mediator.Send(new CustomerDeliveryAddressByCustomerIdQuery.Query(customerId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCustomerDeliveryAddressesByCustomerId), UserAccessor.GetCurrentUserId(), customerId);
                return new BaseResponse<List<CustomerDeliveryAddressModel>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("customer-delivery-address-deletion/{customerDeliveryAddressId}")]
        public async Task<ActionResult<BaseResponse>> DeleteCustomerDeliveryAddress(int customerDeliveryAddressId)
        {
            try
            {
                return await _mediator.Send(new CustomerDeliveryAddressDeletionCommands.Command(customerDeliveryAddressId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteCustomerDeliveryAddress), UserAccessor.GetCurrentUserId(), customerDeliveryAddressId);
                return new BaseResponse(false, ex.Message);
            }
        }
        #endregion

    }
}
