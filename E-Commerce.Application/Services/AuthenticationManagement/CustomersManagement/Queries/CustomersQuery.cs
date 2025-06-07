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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<CustomerModel>>>
        {
            public async Task<BaseResponse<List<CustomerModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CustomerModel> customers = _unitOfWork.GetRepository<Customer>().GetByExpressionAsNoTracking(p =>
                    (string.IsNullOrEmpty(request.SearchText) || p.FirstName.Contains(request.SearchText))
                    && (!request.CityId.HasValue || p.CityId == request.CityId.Value)).Select(s => new CustomerModel
                    {
                        FirstName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.FirstName : s.FirstNameAr,
                        SecondName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.SecondName : s.SecondNameAr,
                        LastName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.LastName : s.LastNameAr,
                        Email = s.Email,
                        MobileNumber = s.MobileNumber,
                        CustomerId = s.CustomerId.ToString(),
                        GenderId = s.GenderId,
                        IsBlock = s.IsBlock,
                        IsActive = s.IsActive,
                        ProfileImage = !request.ReadProfileImage.HasValue || request.ReadProfileImage.Value ? s.ProfileImage : string.Empty,
                        Age = s.DateOfBirth.HasValue ? $"{DateTime.UtcNow.Year - s.DateOfBirth.Value.Year} Y" : string.Empty,
                        Gender = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Gender.Name : s.Gender.NameAr,
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (customers.Any(p => !string.IsNullOrEmpty(p.ProfileImage)) && (!request.ReadProfileImage.HasValue || request.ReadProfileImage.Value))
                    {
                        string DMSCustomerPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCustomerPath];
                        List<string> filePaths = customers.Where(p => !string.IsNullOrEmpty(p.ProfileImage)).Select(s => { return $"{DMSCustomerPath}\\{s.CustomerId.ToLower()}"; }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<CustomerModel>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (CustomerModel customer in customers.Where(p => !string.IsNullOrEmpty(p.ProfileImage)))
                            {
                                customer.ProfileImage = responseGetFileCreationModels.DataObject.FirstOrDefault(p => p.FileName == customer.ProfileImage).FileContent;
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
