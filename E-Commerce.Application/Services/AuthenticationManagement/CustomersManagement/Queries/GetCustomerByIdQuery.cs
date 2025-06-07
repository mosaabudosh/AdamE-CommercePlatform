using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
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

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Queries
{
    public class GetCustomerByIdQuery
    {
        public class Query : IRequest<BaseResponse<CustomerModel>>
        {
            public Guid CustomerId { get; set; }
            public Query(Guid customerId)
            {
                CustomerId = customerId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<CustomerModel>>
        {
            public async Task<BaseResponse<CustomerModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    CustomerModel customer = _unitOfWork.GetRepository<Customer>().GetByExpression(p => p.CustomerId == request.CustomerId).Select(s => new CustomerModel
                    {
                        FirstName = s.FirstName,
                        SecondName = s.SecondName,
                        LastName = s.LastName,
                        FirstNameAr = s.FirstNameAr,
                        SecondNameAr = s.SecondNameAr,
                        LastNameAr = s.LastNameAr,
                        Email = s.Email,
                        MobileNumber = s.MobileNumber,
                        CustomerId = s.CustomerId.ToString(),
                        GenderId = s.GenderId,
                        UserName = s.UserName,
                        DateOfBirth = s.DateOfBirth,
                        Adresse = s.Adresse,
                        CityId = s.CityId,
                        ProfileImage = s.ProfileImage,
                        Gender = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Gender.Name : s.Gender.NameAr,
                        Age = s.DateOfBirth.HasValue ? string.Format("( {0} ) {1}", (DateTime.UtcNow.Year - s.DateOfBirth.Value.Year).ToString(), _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? "Year" : "سنة") : string.Empty
                    }).SingleOrDefault();

                    if (!string.IsNullOrEmpty(customer.ProfileImage))
                    {
                        string DMSCustomerPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCustomerPath];
                        string folderPath = $"{DMSCustomerPath}\\{request.CustomerId.ToString().ToLower()}";
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFolderPathQuery.Query(folderPath));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<CustomerModel>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            customer.FileCreationModel = responseGetFileCreationModels.DataObject.FirstOrDefault();
                        }
                    }

                    return new BaseResponse<CustomerModel>(true, string.Empty, string.Empty, customer);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "GetCustomerByIdQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<CustomerModel>(false, ex.Message);
                }
            }
        }
    }
}
