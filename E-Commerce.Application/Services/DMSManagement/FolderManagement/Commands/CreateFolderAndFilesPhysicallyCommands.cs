using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Domain.Domains;
using E_Commerce.Domain.Domains.DMS;
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

namespace E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands
{
    public class CreateFolderAndFilesPhysicallyCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public Folder Folder { get; set; }
            public FileCreationModel FileCreationModel { get; set; }
            public Command(Folder folder, FileCreationModel fileCreationModel)
            {
                this.Folder = folder;
                this.FileCreationModel = fileCreationModel;

            }
        }
        public class Handler(IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse > Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {                 
                    string pathFromFolderConfiguration = _cachingPlatformSettings[PlatformSettingsCode.DMSRootFolderPath];
                    
                    string path = $"{pathFromFolderConfiguration}{request.Folder.Path}\\{request.Folder.Name}";
                    if (Directory.Exists(path))
                    {
                        if (request.FileCreationModel is not null)
                        {
                            request.FileCreationModel.FileName = $"{request.FileCreationModel.FileName}.{request.FileCreationModel.FileType}";
                            string newPath = $"{path}\\{request.FileCreationModel.FileName}";
                            if (!System.IO.File.Exists(newPath))
                            {
                                byte[] fileContentBytes = await _mediator.Send(new ConvertString64ToByteCommands.Command(request.FileCreationModel.FileContent));
                                System.IO.File.WriteAllBytes(newPath, fileContentBytes);
                            }
                            return new BaseResponse(true);
                        }
                    }
                    else
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                        if (request.FileCreationModel is not null)
                        {
                            request.FileCreationModel.FileName = $"{request.FileCreationModel.FileName}\\{request.FileCreationModel.FileType}";
                            string newPath = $"{path}\\{request.FileCreationModel.FileName}";
                            if (!System.IO.File.Exists(newPath))
                            {
                                byte[] fileContentBytes = await _mediator.Send(new ConvertString64ToByteCommands.Command(request.FileCreationModel.FileContent));
                                System.IO.File.WriteAllBytes(newPath, fileContentBytes);
                            }
                            return new BaseResponse(true);
                        }
                    }
                    return new BaseResponse(false, "The file was not created.");
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CreateFolderAndFilesPhysicallyCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
