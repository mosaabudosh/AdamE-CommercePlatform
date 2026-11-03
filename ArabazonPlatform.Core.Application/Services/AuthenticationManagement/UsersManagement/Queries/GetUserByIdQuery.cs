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
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.UsersManagement.Queries
{
    public class GetUserByIdQuery
    {
        public class Query : IRequest<BaseResponse<UserModel>>
        {
            public Guid UserId { get; set; }
            public Query(Guid userId)
            {
                UserId = userId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<UserModel>>
        {
            public async Task<BaseResponse<UserModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    UserModel user = _unitOfWork.GetRepository<User>().GetByExpression(p => p.UserId == request.UserId).Select(s => new UserModel
                    {
                        FirstName = s.FirstName,
                        SecondName = s.SecondName,
                        LastName = s.LastName,
                        FirstNameAr = s.FirstNameAr,
                        SecondNameAr = s.SecondNameAr,
                        LastNameAr = s.LastNameAr,
                        Email = s.Email,
                        MobileNumber = s.MobileNumber,
                        UserId = s.UserId.ToString(),
                        GenderId = s.GenderId,
                        UserName = s.UserName,
                        DateOfBirth = s.DateOfBirth,
                        Adresse = s.Adresse,
                        CityId = s.CityId,
                        UserTypeId = s.UserTypeId,
                        ProfileImage = s.ProfileImage,
                        ShopId = s.ShopId,
                        Gender = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Gender.Name : s.Gender.NameAr,
                        Age = s.DateOfBirth.HasValue ? string.Format("( {0} ) {1}", (DateTime.UtcNow.Year - s.DateOfBirth.Value.Year).ToString(), _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? "Year" : "سنة") : string.Empty
                    }).SingleOrDefault();

                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSUserPath, $"{_cachingPlatformSettings[PlatformSettingsCode.DMSUserPath]}\\{request.UserId.ToString().ToLower()}"];
                        if (responseGetFileCreationModels.Any())
                        {
                            user.FileCreationModel = responseGetFileCreationModels.FirstOrDefault();
                        }
                    }

                    return new BaseResponse<UserModel>(true, string.Empty, string.Empty, user);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "GetUserByIdQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<UserModel>(false, ex.Message);
                }
            }
        }
    }
}
