using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.Enum;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands
{
    public class RegistrationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public UserModel UserModel { get; set; }

            public string UserPathDMS = "Users";
            public Command(UserModel userModel)
            {
                UserModel = userModel;
            }

            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, IAuthenticationService _authenticationService) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        IQueryable<User> usersQueryable = _unitOfWork.GetRepository<User>().GetByExpression(x => x.UserName == request.UserModel.UserName, x => x.OrderBy(z => z.UserName));
                        if (usersQueryable.Any())
                        {
                            return new BaseResponse(false, string.Empty, ErrorCode.AlreadyExsist);
                        }
                        User user = new User()
                        {
                            UserName = request.UserModel.UserName,
                            FirstName = request.UserModel.FirstName,
                            FirstNameAr = request.UserModel.FirstNameAr,
                            SecondName = request.UserModel.SecondName,
                            SecondNameAr = request.UserModel.SecondNameAr,
                            LastName = request.UserModel.LastName,
                            LastNameAr = request.UserModel.LastNameAr,
                            Email = request.UserModel.Email,
                            MobileNumber = request.UserModel.MobileNumber,
                            LanguageId = request.UserModel.LanguageId,
                            GenderId = request.UserModel.GenderId,
                            Adresse = request.UserModel.Adresse,
                            CityId = request.UserModel.CityId,
                            DateOfBirth = request.UserModel.DateOfBirth,
                            IsBlock = request.UserModel.IsBlock.HasValue ? request.UserModel.IsBlock.Value : false,
                            UserTypeId = request.UserModel.UserTypeId,
                            ProfileImage = !string.IsNullOrEmpty(request.UserModel.ProfileImage) ? request.UserModel.ProfileImage.Trim() : null,
                            UserRoles = new List<UserRole>(),
                            UserSettings = new List<UserSetting>(),
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        };
                        byte[] passwordHash, passwordSalt;
                        _authenticationService.HashPassword(request.UserModel.Password, out passwordHash, out passwordSalt);
                        user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;
                        user.UserRoles.Add(new UserRole
                        {
                            RoleId = (int)Roles.User,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        });
                        user.UserSettings.AddRange(new List<UserSetting>()
                        {
                            new UserSetting()
                            {
                                PlatformSettingId = (int)PlatformSettings.DefaultTheme,
                                Value = PlatformTheme.NormalMode,
                                CreationUserId = _userAccessor.GetCurrentUserId(),
                                CreationDate = DateTime.UtcNow,
                                IsActive = true
                            },
                            new UserSetting()
                            {
                                PlatformSettingId = (int)PlatformSettings.DefaultLanguage,
                                Value = Convert.ToInt32(Enums.Languages.English).ToString(),
                                CreationUserId = _userAccessor.GetCurrentUserId(),
                                CreationDate = DateTime.UtcNow,
                                IsActive = true
                            },
                            new UserSetting()
                            {
                                PlatformSettingId = (int)PlatformSettings.DefaultPage,
                                Value = $"{Enums.Screens.Welcome}",
                                CreationUserId = _userAccessor.GetCurrentUserId(),
                                CreationDate = DateTime.UtcNow,
                                IsActive = true
                            },
                            new UserSetting()
                            {
                                PlatformSettingId = (int)PlatformSettings.DefaultMenuType,
                                Value = $"{Enums.MenuType.Open}",
                                CreationUserId = _userAccessor.GetCurrentUserId(),
                                CreationDate = DateTime.UtcNow,
                                IsActive = true
                            },
                        });
                        _unitOfWork.GetRepository<User>().Insert(user);
                        await _unitOfWork.CommitChanges();

                        if (request.UserModel.FileCreationModel is not null)
                        {
                            FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                            {
                                FolderName = $"{user.UserId}",
                                FolderPath = $"{request.UserPathDMS}",
                                FileCreationModels = new List<FileCreationModel>()
                            };
                            folderAndFileCreationModel.FileCreationModels.Add(request.UserModel.FileCreationModel);
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
                        await _exceptionService.LogErrorAsync(ex, "RegistrationCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse(false, ex.Message);
                    }
                }
            }

        }
    }
}
