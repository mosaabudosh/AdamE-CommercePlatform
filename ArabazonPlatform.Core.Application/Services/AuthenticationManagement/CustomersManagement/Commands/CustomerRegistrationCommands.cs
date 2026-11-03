using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Authentication.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Domains.Authorisation;
using ArabazonPlatform.Domain.Enum;
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

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.CustomersManagement.Commands
{
    public class CustomerRegistrationCommands
    {
        public class Command : IRequest<BaseResponse<CustomerRegisterModel>>
        {
            public CustomerModel CustomerModel { get; set; }
            public Command(CustomerModel customerModel)
            {
                CustomerModel = customerModel;
            }

            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, IAuthenticationService _authenticationService, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse<CustomerRegisterModel>>
            {
                public async Task<BaseResponse<CustomerRegisterModel>> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        CustomerRegisterModel customerRegisterModel = new CustomerRegisterModel();
                        string DMSCustomerPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCustomerPath];

                        IQueryable<Customer> customersQueryable = _unitOfWork.GetRepository<Customer>().GetByExpression(x => x.UserName == request.CustomerModel.UserName, x => x.OrderBy(z => z.UserName));

                        if (customersQueryable.Any())
                        {
                            Customer exsistCustomer = await _unitOfWork.GetRepository<Customer>().FirstOrDefaultAsync(customersQueryable);
                            customerRegisterModel = new CustomerRegisterModel()
                            {
                                CustomerId = exsistCustomer.CustomerId,
                                Email = exsistCustomer.Email,
                                UserName = exsistCustomer.UserName,
                            };
                            return new BaseResponse<CustomerRegisterModel>(false, string.Empty, ArabazonPlatform.Generic.Constants.Constants.ErrorCode.AlreadyExsist, customerRegisterModel);
                        }


                        Customer customer = new Customer()
                        {
                            UserName = request.CustomerModel.Email,
                            FirstName = request.CustomerModel.FirstName,
                            FirstNameAr = request.CustomerModel.FirstNameAr,
                            SecondName = request.CustomerModel.SecondName,
                            SecondNameAr = request.CustomerModel.SecondNameAr,
                            LastName = request.CustomerModel.LastName,
                            LastNameAr = request.CustomerModel.LastNameAr,
                            Email = request.CustomerModel.Email,
                            MobileNumber = request.CustomerModel.MobileNumber,
                            GenderId = request.CustomerModel.GenderId,
                            Adresse = request.CustomerModel.Adresse,
                            CityId = request.CustomerModel.CityId,
                            DateOfBirth = request.CustomerModel.DateOfBirth,
                            UserTypeId = (int)Enums.UserTypes.Customer,
                            IsBlock = request.CustomerModel.IsBlock.HasValue ? request.CustomerModel.IsBlock.Value : false,
                            ProfileImage = !string.IsNullOrEmpty(request.CustomerModel.ProfileImage) ? request.CustomerModel.ProfileImage.Trim() : null,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        };
                        if (!string.IsNullOrEmpty(request.CustomerModel.Password))
                        {
                            byte[] passwordHash, passwordSalt;
                            _authenticationService.HashPassword(request.CustomerModel.Password, out passwordHash, out passwordSalt);
                            customer.PasswordHash = passwordHash;
                            customer.PasswordSalt = passwordSalt;
                        }
                        else
                        {
                            customer.OTPCode = await _mediator.Send(new GenerateOTPCodeCommands.Command());
                            customerRegisterModel = new CustomerRegisterModel()
                            {
                                CustomerId = customer.CustomerId,
                                Email = customer.Email,
                                UserName = customer.UserName,
                            };
                        }

                        _unitOfWork.GetRepository<Customer>().Insert(customer);
                        await _unitOfWork.CommitChanges();
                        if (request.CustomerModel.FileCreationModel is not null)
                        {
                            FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                            {
                                FolderName = $"{customer.CustomerId}",
                                FolderPath = $"{DMSCustomerPath}",
                                FileCreationModels = new List<FileCreationModel>()
                            };
                            folderAndFileCreationModel.FileCreationModels.Add(request.CustomerModel.FileCreationModel);
                            BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                            if (!createFolderAndOneFileResponse.IsSuccess)
                            {
                                return new BaseResponse<CustomerRegisterModel>(false, createFolderAndOneFileResponse.Message,createFolderAndOneFileResponse.ErrorCode,null);
                            }
                        }

                        customerRegisterModel.CustomerId = customer.CustomerId;

                        return new BaseResponse<CustomerRegisterModel>(true, string.Empty, customerRegisterModel);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "CustomerRegistrationCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse<CustomerRegisterModel>(false, ex.Message);
                    }
                }

            }
        }
    }
}
