using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Domain.Domains;
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

namespace E_Commerce.Application.Services.DashBoardManagement.CountriesManagement.Commands
{
    public class CountryCreationCommands
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
                Domain.Domains.Country country = new Domain.Domains.Country();
                try
                {
                    country = new Domain.Domains.Country()
                    {
                        Name = request.CountryModel.Name.Trim(),
                        NameAr = request.CountryModel.NameAr.Trim(),
                        Banner = request.CountryModel.Banner,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    _unitOfWork.GetRepository<Domain.Domains.Country>().Insert(country);
                    await _unitOfWork.CommitChanges();

                    if (request.CountryModel.FileCreationModel is not null)
                    {
                        FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                        {
                            FolderName = $"{country.Name.Trim()}",
                            FolderPath = $"{DMSCountryPath}",
                            FileCreationModels = new List<FileCreationModel>(),
                        };
                        folderAndFileCreationModel.FileCreationModels.Add(request.CountryModel.FileCreationModel);
                        BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                        if (!createFolderAndOneFileResponse.IsSuccess)
                        {
                            return createFolderAndOneFileResponse;
                        }
                    }
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    if (request.CountryModel.FileCreationModel is not null)
                    {
                        string folderPath = $"{DMSCountryPath}\\{country.Name.Trim()}";
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
                    await _exceptionService.LogErrorAsync(ex, "CountryCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
