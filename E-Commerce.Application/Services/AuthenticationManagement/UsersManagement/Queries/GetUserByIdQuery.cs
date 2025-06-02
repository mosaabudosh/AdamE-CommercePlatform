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
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Queries
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<UserModel>>
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
                        Gender = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Gender.Name : s.Gender.NameAr,
                        Age = s.DateOfBirth.HasValue ? string.Format("( {0} ) {1}", (DateTime.UtcNow.Year - s.DateOfBirth.Value.Year).ToString(), _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? "Year" : "سنة") : string.Empty
                    }).SingleOrDefault();

                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        string DMSUserPath = _cachingPlatformSettings[PlatformSettingsCode.DMSUserPath];
                        string folderPath = $"{DMSUserPath}\\{request.UserId.ToString().ToLower()}";
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFolderPathQuery.Query(folderPath));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<UserModel>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            user.FileCreationModel = responseGetFileCreationModels.DataObject.FirstOrDefault();
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
