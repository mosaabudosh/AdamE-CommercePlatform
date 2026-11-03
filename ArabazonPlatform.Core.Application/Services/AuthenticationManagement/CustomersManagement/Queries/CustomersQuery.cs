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
    public class CustomersQuery
    {
        public class Query : IRequest<BaseResponse<List<CustomerModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }
            public int? CityId { get; set; }
            public bool? ReadProfileImage { get; set; }

            public Query(int pageNumber, int pageSize, string? searchText = null, int? cityId = null, bool? readProfileImage = false)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
                CityId = cityId;
                ReadProfileImage = readProfileImage;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<List<CustomerModel>>>
        {
            public async Task<BaseResponse<List<CustomerModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CustomerModel> customers = _unitOfWork.GetRepository<Customer>().GetByExpressionAsNoTracking(p =>
                    (string.IsNullOrEmpty(request.SearchText) || p.FirstName.Contains(request.SearchText))
                    && (!request.CityId.HasValue || p.CityId == request.CityId.Value)).Select(s => new CustomerModel
                    {
                        FirstName = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.FirstName : s.FirstNameAr,
                        SecondName = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.SecondName : s.SecondNameAr,
                        LastName = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.LastName : s.LastNameAr,
                        Email = s.Email,
                        MobileNumber = s.MobileNumber,
                        CustomerId = s.CustomerId.ToString(),
                        GenderId = s.GenderId,
                        IsBlock = s.IsBlock,
                        IsActive = s.IsActive,
                        ProfileImage = !request.ReadProfileImage.HasValue || request.ReadProfileImage.Value ? s.ProfileImage : string.Empty,
                        Age = s.DateOfBirth.HasValue ? $"{DateTime.UtcNow.Year - s.DateOfBirth.Value.Year} Y" : string.Empty,
                        Gender = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Gender.Name : s.Gender.NameAr,
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (customers.Any(p => !string.IsNullOrEmpty(p.ProfileImage)) && (!request.ReadProfileImage.HasValue || request.ReadProfileImage.Value))
                    {
                        List<string> filePaths = customers.Where(p => !string.IsNullOrEmpty(p.ProfileImage)).Select(s => { return $"{_cachingPlatformSettings[PlatformSettingsCode.DMSCustomerPath]}\\{s.CustomerId.ToLower()}"; }).ToList();

                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSCustomerPath, filePaths];
                        if (responseGetFileCreationModels.Any())
                        {
                            foreach (CustomerModel customer in customers.Where(p => !string.IsNullOrEmpty(p.ProfileImage)))
                            {
                                customer.ProfileImage = responseGetFileCreationModels.FirstOrDefault(p => p.FileName == customer.ProfileImage).FileContent;
                            }
                        }
                    }
                    return new BaseResponse<List<CustomerModel>>(true, string.Empty, string.Empty, customers);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CustomersQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<CustomerModel>>(false, ex.Message);
                }
            }
        }
    }
}
