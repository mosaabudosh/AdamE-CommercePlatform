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
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands
{
    public class UserUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public UserModel UserModel { get; set; }
            public Command(UserModel userModel)
            {
                UserModel = userModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                string DMSUserPath = _cachingPlatformSettings[PlatformSettingsCode.DMSUserPath];

                try
                {
                    Guid userId = new Guid(request.UserModel.UserId);
                    User user = _unitOfWork.GetRepository<User>().GetByExpressionAsNoTracking(x => x.UserId == userId).SingleOrDefault();
                    user.FirstName = request.UserModel.FirstName.Trim();
                    user.FirstNameAr = request.UserModel.FirstNameAr.Trim();
                    user.SecondName = request.UserModel.SecondName.Trim();
                    user.SecondNameAr = request.UserModel.SecondNameAr.Trim();
                    user.LastName = request.UserModel.LastName.Trim();
                    user.LastNameAr = request.UserModel.LastNameAr.Trim();
                    user.Email = request.UserModel.Email.Trim();
                    user.MobileNumber = request.UserModel.MobileNumber.Trim();
                    user.CityId = request.UserModel.CityId;
                    user.LanguageId = request.UserModel.LanguageId;
                    user.GenderId = request.UserModel.GenderId;
                    user.DateOfBirth = request.UserModel.DateOfBirth;
                    user.ProfileImage = !string.IsNullOrEmpty(request.UserModel.ProfileImage) ? request.UserModel.ProfileImage.Trim() : null;
                    user.LastUpdatedDate = DateTime.UtcNow;
                    user.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    _unitOfWork.GetRepository<User>().Update(user);
                    await _unitOfWork.CommitChanges();

                    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    {
                        FolderName = $"{user.UserId}",
                        FolderPath = $"{DMSUserPath}",
                        FileCreationModels = new List<FileCreationModel>(),
                    };

                    if (request.UserModel.FileCreationModel is not null)
                    {
                        folderAndFileCreationModel.FileCreationModels.Add(request.UserModel.FileCreationModel);
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
                    if (request.UserModel.FileCreationModel is not null)
                    {
                        string folderPath = $"{DMSUserPath}\\{request.UserModel.UserId}";
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
                    await _exceptionService.LogErrorAsync(ex, "UserUpdationCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
