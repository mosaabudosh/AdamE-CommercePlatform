using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Domains.DashBoard;
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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.CountriesManagement.Commands
{
    public class CountryUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CountryModel CountryModel { get; set; }
            public Command(CountryModel countryModel)
            {
                CountryModel = countryModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                string DMSCountryPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCountryPath];
                try
                {
                    Country country = await _unitOfWork.GetRepository<Country>().GetById(request.CountryModel.CountryId);
                    country.Name = request.CountryModel.Name.Trim();
                    country.NameAr = request.CountryModel.NameAr.Trim();
                    country.Banner = request.CountryModel.Banner;
                    country.LastUpdatedDate = DateTime.UtcNow;
                    country.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    await _unitOfWork.CommitChanges();

                    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    {
                        FolderName = $"{country.Name.Trim()}",
                        FolderPath = $"{DMSCountryPath}",
                        FileCreationModels = new List<FileCreationModel>(),
                    };
                    if (request.CountryModel.FileCreationModel is not null)
                    {
                        folderAndFileCreationModel.FileCreationModels.Add(request.CountryModel.FileCreationModel);
                    }
                    BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                    if (!createFolderAndOneFileResponse.IsSuccess)
                    {
                        return createFolderAndOneFileResponse;
                    }

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    if (request.CountryModel.FileCreationModel is not null)
                    {
                        string folderPath = $"{DMSCountryPath}\\{request.CountryModel.Name.Trim()}";
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
                    await _exceptionService.LogErrorAsync(ex, "CountryUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
