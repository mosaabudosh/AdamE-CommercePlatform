using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
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

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Commands
{
    public class CustomerUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CustomerModel CustomerModel { get; set; }
            public Command(CustomerModel customerModel)
            {
                CustomerModel = customerModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _customerAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                string DMSCustomerPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCustomerPath];

                try
                {
                    Guid customerId = new Guid(request.CustomerModel.CustomerId);
                    Customer customer = _unitOfWork.GetRepository<Customer>().GetByExpressionAsNoTracking(x => x.CustomerId == customerId).SingleOrDefault();
                    customer.FirstName = request.CustomerModel.FirstName.Trim();
                    customer.FirstNameAr = request.CustomerModel.FirstNameAr.Trim();
                    customer.SecondName = request.CustomerModel.SecondName.Trim();
                    customer.SecondNameAr = request.CustomerModel.SecondNameAr.Trim();
                    customer.LastName = request.CustomerModel.LastName.Trim();
                    customer.LastNameAr = request.CustomerModel.LastNameAr.Trim();
                    customer.Email = request.CustomerModel.Email.Trim();
                    customer.MobileNumber = request.CustomerModel.MobileNumber.Trim();
                    customer.CityId = request.CustomerModel.CityId;
                    customer.GenderId = request.CustomerModel.GenderId;
                    customer.DateOfBirth = request.CustomerModel.DateOfBirth;
                    customer.ProfileImage = !string.IsNullOrEmpty(request.CustomerModel.ProfileImage) ? request.CustomerModel.ProfileImage.Trim() : null;
                    customer.LastUpdatedDate = DateTime.UtcNow;
                    customer.LastUpdateUserId = _customerAccessor.GetCurrentUserId();

                    _unitOfWork.GetRepository<Customer>().Update(customer);
                    await _unitOfWork.CommitChanges();

                    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    {
                        FolderName = $"{customer.CustomerId}",
                        FolderPath = $"{DMSCustomerPath}",
                        FileCreationModels = new List<FileCreationModel>(),
                    };

                    if (request.CustomerModel.FileCreationModel is not null)
                    {
                        folderAndFileCreationModel.FileCreationModels.Add(request.CustomerModel.FileCreationModel);
                    }
                    BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                    if (!createFolderAndOneFileResponse.IsSuccess)
                    {
                        return createFolderAndOneFileResponse;
                    }
                    return new BaseResponse(true, string.Empty);
                }
                catch (Exception ex)
                {
                    if (request.CustomerModel.FileCreationModel is not null)
                    {
                        string folderPath = $"{DMSCustomerPath}\\{request.CustomerModel.CustomerId}";
                        BaseResponse responseGetFileCreationModels = await _mediator.Send(new DeleteFolderByPathCommands.Command(folderPath));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return responseGetFileCreationModels;
                        }
                    }

                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CustomerUpdationCommands", _customerAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
