using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
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
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Commands
{
    public class CustomerRegistrationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CustomerModel CustomerModel { get; set; }
            public Command(CustomerModel customerModel)
            {
                CustomerModel = customerModel;
            }

            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, IAuthenticationService _authenticationService, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        string DMSCustomerPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCustomerPath];

                        IQueryable<Customer> usersQueryable = _unitOfWork.GetRepository<Customer>().GetByExpression(x => x.UserName == request.CustomerModel.UserName, x => x.OrderBy(z => z.UserName));
                        if (usersQueryable.Any())
                        {
                            return new BaseResponse(false, string.Empty, Adam.Generic.Constants.Constants.ErrorCode.AlreadyExsist);
                        }
                        Customer user = new Customer()
                        {
                            UserName = request.CustomerModel.UserName,
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
                            IsBlock = request.CustomerModel.IsBlock.HasValue ? request.CustomerModel.IsBlock.Value : false,
                            ProfileImage = !string.IsNullOrEmpty(request.CustomerModel.ProfileImage) ? request.CustomerModel.ProfileImage.Trim() : null,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        };
                        byte[] passwordHash, passwordSalt;
                        _authenticationService.HashPassword(request.CustomerModel.Password, out passwordHash, out passwordSalt);
                        user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;
                  
                        _unitOfWork.GetRepository<Customer>().Insert(user);
                        await _unitOfWork.CommitChanges();

                        if (request.CustomerModel.FileCreationModel is not null)
                        {
                            FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                            {
                                FolderName = $"{user.CustomerId}",
                                FolderPath = $"{DMSCustomerPath}",
                                FileCreationModels = new List<FileCreationModel>()
                            };
                            folderAndFileCreationModel.FileCreationModels.Add(request.CustomerModel.FileCreationModel);
                            BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                            if (!createFolderAndOneFileResponse.IsSuccess)
                            {
                                return createFolderAndOneFileResponse;
                            }
                        }

                        return new BaseResponse(true, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "CustomerRegistrationCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse(false, ex.Message);
                    }
                }
            }

        }
    }
}
