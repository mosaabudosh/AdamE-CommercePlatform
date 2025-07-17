using Adam.Logging.Interfaces;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using Adam.Generic.Generic;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Application.Caching;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Queries
{
    public class UsersQuery
    {
        public class Query : IRequest<BaseResponse<List<UserModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }
            public int? CityId { get; set; }
            public int? ShopId { get; set; }
            public int? LanguageId { get; set; }
            public bool? ReadProfileImage { get; set; }


            public Query(int pageNumber, int pageSize, string? searchText = null, int? cityId = null, int? shopId = null, int? languageId = null, bool? readProfileImage = false)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
                CityId = cityId;
                ShopId = shopId;
                LanguageId = languageId;
                ReadProfileImage = readProfileImage;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<UserModel>>>
        {
            public async Task<BaseResponse<List<UserModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<UserModel> users = _unitOfWork.GetRepository<User>().GetByExpressionAsNoTracking(p =>
                    (string.IsNullOrEmpty(request.SearchText) || p.FirstName.Contains(request.SearchText))
                    && (!request.CityId.HasValue || p.CityId == request.CityId.Value)
                    && (!request.ShopId.HasValue || p.ShopId == request.ShopId.Value)).Select(s => new UserModel
                    {
                        FirstName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.FirstName : s.FirstNameAr,
                        SecondName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.SecondName : s.SecondNameAr,
                        LastName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.LastName : s.LastNameAr,
                        Email = s.Email,
                        MobileNumber = s.MobileNumber,
                        UserId = s.UserId.ToString(),
                        GenderId = s.GenderId,
                        IsBlock = s.IsBlock,
                        IsActive = s.IsActive,
                        ShopId = s.ShopId,
                        CityId = s.CityId,
                        UserTypeId = s.UserTypeId,
                        ProfileImage = !request.ReadProfileImage.HasValue || request.ReadProfileImage.Value ? s.ProfileImage : string.Empty,
                        Age = s.DateOfBirth.HasValue ? $"{DateTime.UtcNow.Year - s.DateOfBirth.Value.Year} Y" :string.Empty,
                        Gender = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Gender.Name : s.Gender.NameAr,
                        UserRoles = s.UserRoles.Select(r => new UserRoleModel()
                        {
                            UserRoleId = r.UserRoleId,
                            RoleId = r.RoleId,
                            UserId = r.UserId,
                            Role = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? r.Role.Name : r.Role.NameAr,
                        }).ToList()
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (users.Any(p => !string.IsNullOrEmpty(p.ProfileImage)) && (!request.ReadProfileImage.HasValue || request.ReadProfileImage.Value))
                    {
                        string DMSUserPath = _cachingPlatformSettings[PlatformSettingsCode.DMSUserPath];
                        List<string> filePaths = users.Where(p => !string.IsNullOrEmpty(p.ProfileImage)).Select(s => { return $"{DMSUserPath}\\{s.UserId.ToLower()}"; }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<UserModel>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (UserModel user in users.Where(p => !string.IsNullOrEmpty(p.ProfileImage)))
                            {
                                user.ProfileImage = responseGetFileCreationModels.DataObject.FirstOrDefault(p => p.FileName == user.ProfileImage).FileContent;
                            }
                        }
                    }
                    return new BaseResponse<List<UserModel>>(true, string.Empty, string.Empty, users);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UsersQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<UserModel>>(false, ex.Message);
                }
            }
        }
    }
}
