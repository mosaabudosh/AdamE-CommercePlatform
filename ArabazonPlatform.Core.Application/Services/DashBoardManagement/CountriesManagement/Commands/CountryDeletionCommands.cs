using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
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
    public class CountryDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int CountryId { get; set; }
            public Command(int countryId)
            {
                CountryId = countryId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Country country = await _unitOfWork.GetRepository<Country>().GetById(request.CountryId);
                    _unitOfWork.GetRepository<Country>().Delete(country);
                    if (!string.IsNullOrEmpty(country.Banner))
                    {
                        string DMSCountryPath = _cachingPlatformSettings[PlatformSettingsCode.DMSCountryPath];
                        string folderPath = $"{DMSCountryPath}\\{country.Name.Trim()}";
                        BaseResponse responseGetFileCreationModels = await _mediator.Send(new DeleteFolderByPathCommands.Command(folderPath, false));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return responseGetFileCreationModels;
                        }
                    }
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CountryDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
