using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains;
using ArabazonPlatform.Domain.Domains.DMS;
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
using File = ArabazonPlatform.Domain.Domains.DMS.File;

namespace ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands
{
    public class CreateFolderAndFilesWithoutDeleteCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public List<FileCreationModel> FileCreationModels { get; set; }
            public Folder Folder { get; set; }

            public Command(List<FileCreationModel> fileCreationModels, Folder folder)
            {
                this.FileCreationModels = fileCreationModels;
                this.Folder = folder;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                string pathFromFolderConfiguration = _cachingPlatformSettings[PlatformSettingsCode.DMSRootFolderPath];
                try
                {
                    string folderPath = $"{pathFromFolderConfiguration}{request.Folder.Path}\\{request.Folder.Name}";
                    Folder existfolder = _unitOfWork.GetRepository<Folder>().GetByExpression(x => x.Name == request.Folder.Name && x.Path == folderPath, includedEntities: new List<System.Linq.Expressions.Expression<Func<Folder, object>>>() { z => z.Files }).FirstOrDefault();

                    string path = $"{request.Folder.Path}\\{request.Folder.Name}";

                    if (request.FileCreationModels.Any())
                    {
                        BaseResponse<List<File>> filesResponse = await _mediator.Send(new GenerateFilesCommands.Command(request.Folder, request.FileCreationModels, folderPath));
                        if (!filesResponse.IsSuccess)
                        {
                            return new BaseResponse(false, filesResponse.Message);
                        }
                        request.Folder.Files = new List<File>(filesResponse.DataObject);
                        request.Folder.CreationUserId = _userAccessor.GetCurrentUserId();
                        request.Folder.IsActive = true;
                        request.Folder.CreationDate = DateTime.UtcNow;

                        string[] paths = path.Split("\\");
                        List<Folder> folders = new List<Folder>();
                        List<Folder> newFolders = new List<Folder>();
                        if (existfolder is null)
                        {
                            BaseResponse<List<Folder>> responseFolder = await _mediator.Send(new GenerateFoldersCommands.Command(folders, newFolders, request.Folder, paths, pathFromFolderConfiguration));
                            if (!responseFolder.IsSuccess)
                            {
                                return new BaseResponse(false, responseFolder.Message);
                            }
                            _unitOfWork.GetRepository<Folder>().AddRange(responseFolder.DataObject);
                            await _unitOfWork.CommitChanges();
                            return new BaseResponse(true);
                        }
                        else
                        {
                            existfolder.Files.AddRange(filesResponse.DataObject);
                            _unitOfWork.GetRepository<Folder>().Update(existfolder);
                            await _unitOfWork.CommitChanges();
                            return new BaseResponse(true);
                        }
                    }
                    else
                    {
                        request.Folder.Path = $"{pathFromFolderConfiguration}{request.Folder.Path}\\{request.Folder.Name}";
                        BaseResponse<List<Folder>> folders = await _mediator.Send(new FolderByPathQuery.Query(request.Folder.Path));
                        if (!folders.IsSuccess)
                        {
                            return new BaseResponse(false, folders.Message);
                        }
                        if (folders.DataObject.Any())
                        {
                            folders.DataObject.ForEach(async oldFolder =>
                            {
                                BaseResponse responseDelete = await _mediator.Send(new DeleteFolderCommands.Command(oldFolder.FolderId));
                            });
                        }
                        return new BaseResponse(true);
                    }
                }
                catch (Exception ex)
                {
                    request.Folder.Path = $"{pathFromFolderConfiguration}{request.Folder.Path}\\{request.Folder.Name}";
                    BaseResponse<List<Folder>> folders = await _mediator.Send(new FolderByPathQuery.Query(request.Folder.Path));
                    if (!folders.IsSuccess)
                    {
                        return new BaseResponse(false, folders.Message);
                    }
                    if (folders.DataObject.Any())
                    {
                        folders.DataObject.ForEach(async oldFolder =>
                        {
                            BaseResponse responseDelete = await _mediator.Send(new DeleteFolderCommands.Command(oldFolder.FolderId));
                        });
                    }
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CreateFolderAndFilesWithoutDeleteCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(true, ex.Message);
                }
            }
        }

    }
}
