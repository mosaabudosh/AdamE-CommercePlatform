using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
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

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.CustomersManagement.Queries
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<CustomerModel>>
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
                        Gender = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Gender.Name : s.Gender.NameAr,
                        Age = s.DateOfBirth.HasValue ? string.Format("( {0} ) {1}", (DateTime.UtcNow.Year - s.DateOfBirth.Value.Year).ToString(), _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? "Year" : "سنة") : string.Empty
                    }).SingleOrDefault();

                    if (!string.IsNullOrEmpty(customer.ProfileImage))
                    {
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSCustomerPath, $"{_cachingPlatformSettings[PlatformSettingsCode.DMSCustomerPath]}\\{customer.CustomerId.ToString().ToLower()}"];
                        if (responseGetFileCreationModels.Any())
                        {
                            customer.FileCreationModel = responseGetFileCreationModels.FirstOrDefault();
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
