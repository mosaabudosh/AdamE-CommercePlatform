using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DMS;
using E_Commerce.Application.Services.DMSManagement.FileType.Queries;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
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
using File = E_Commerce.Domain.Domains.DMS.File;

namespace E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands
{
    public class CreateFolderAndOneFileCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public FolderAndFileCreationModel FolderAndFileCreationModel { get; set; }
            public Command(FolderAndFileCreationModel folderAndFileCreationModel)
            {
                FolderAndFileCreationModel = folderAndFileCreationModel;
            }
        }
        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                string pathFromFolderConfiguration = _cachingPlatformSettings[PlatformSettingsCode.DMSRootFolderPath];

                Folder folder = new Folder()
                {
                    Name = request.FolderAndFileCreationModel.FolderName,
                    Path = request.FolderAndFileCreationModel.FolderPath,
                };
                try
                {
                    string folderPath = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                    string path = $"{folder.Path}\\{folder.Name}";
                    BaseResponse<List<Folder>> oldfolders = await _mediator.Send(new FolderByPathQuery.Query($"{pathFromFolderConfiguration}{path}"));
                    if (!oldfolders.IsSuccess)
                    {
                        return new BaseResponse(false, oldfolders.Message);
                    }
                    if (request.FolderAndFileCreationModel.FileCreationModels.Any() && oldfolders.DataObject.Any())
                    {
                        foreach (Folder oldFolder in oldfolders.DataObject)
                        {
                            if (oldFolder.Name == folder.Name)
                            {
                                BaseResponse responseDelete = await _mediator.Send(new DeleteFolderCommands.Command(oldFolder.FolderId));
                                //if (oldFolder.Files.Any())
                                //{
                                //    _unitOfWork.GetRepository<File>().RemoveRange(oldFolder.Files);
                                //}
                                //_unitOfWork.GetRepository<Folder>().Delete(oldFolder);
                                //await _unitOfWork.CommitChanges();
                                //BaseResponse deletePhsiycalFolderCommandsResponse = await _mediator.Send(new DeletePhsiycalFolderCommands.Command(oldFolder.Path));
                                //if (!deletePhsiycalFolderCommandsResponse.IsSuccess)
                                //{
                                //    throw new Exception(deletePhsiycalFolderCommandsResponse.Message);
                                //}
                            }
                        }
                  
                    }

                    IQueryable<File> filesQuery = _unitOfWork.GetRepository<File>().GetByExpression(x => x.Path.Contains(folderPath),
                    includedEntities: new List<System.Linq.Expressions.Expression<Func<File, object>>>() { z => z.Folder });
                    List<File> oldfiles = await _unitOfWork.GetRepository<File>().ToListAsync(filesQuery);
                    if (oldfiles.Any())
                    {
                        Folder oldfolder = filesQuery.FirstOrDefault().Folder;
                        if (oldfolder != null)
                        {
                            _unitOfWork.GetRepository<File>().RemoveRange(oldfolder.Files);
                            await _mediator.Send(new DeletePhsiycalFolderCommands.Command(folderPath));
                        }
                    }

                    if (request.FolderAndFileCreationModel.FileCreationModels.Any())
                    {
                        BaseResponse<List<File>> filesResponse = await _mediator.Send(new GenerateFilesCommands.Command(folder, request.FolderAndFileCreationModel.FileCreationModels, folderPath));
                        if (!filesResponse.IsSuccess)
                        {
                            return new BaseResponse(false, filesResponse.Message);
                        }
                        folder.Files = new List<File>(filesResponse.DataObject);
                        folder.CreationUserId = _userAccessor.GetCurrentUserId();
                        folder.CreationDate = DateTime.UtcNow;
                        folder.IsActive = true;

                        string[] paths = path.Split("\\");
                        List<Folder> folders = new List<Folder>();
                        List<Folder> newFolders = new List<Folder>();
                        if (paths.Any())
                        {
                            int i = 0;
                            foreach (string newPath in paths)
                            {
                                if (i <= paths.Length - 2)
                                {
                                    Folder newFolder = new Folder();
                                    newFolder.Path = (i == 0) ? pathFromFolderConfiguration + newPath : folders[i - 1].Path + "\\" + newPath;
                                    newFolder.CreationUserId = _userAccessor.GetCurrentUserId();
                                    newFolder.IsActive = true;
                                    newFolder.Name = newPath;
                                    newFolder.CreationDate = DateTime.UtcNow;
                                    newFolder.ParentFolderId = (i > 0) ? folders[i - 1].FolderId : newFolder.ParentFolderId;
                                    newFolder.ParentFolder = (i > 0) ? folders[i - 1] : null;

                                    Folder existFolder = _unitOfWork.GetRepository<Folder>().GetByExpression(x => x.Path.Contains(newFolder.Path)).FirstOrDefault();
                                    if (existFolder == null)
                                    {
                                        folders.Add(newFolder);
                                        newFolders.Add(newFolder);
                                    }
                                    else
                                    {
                                        folders.Add(existFolder);
                                    }
                                }
                                i++;
                            }
                        }
                        folder.ParentFolder = folders.LastOrDefault();
                        folder.ParentFolderId = folders.LastOrDefault().FolderId;
                        folder.Path = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";

                        _unitOfWork.GetRepository<Folder>().Insert(folder);

                        await _unitOfWork.CommitChanges();

                        return new BaseResponse(true);
                    }
                    else
                    {
                        folder.Path = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                        BaseResponse<List<Folder>> folders = await _mediator.Send(new FolderByPathQuery.Query(folder.Path));
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
                        return new BaseResponse(true, string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    folder.Path = $"{pathFromFolderConfiguration}{folder.Path}";
                    BaseResponse<List<Folder>> folders = await _mediator.Send(new FolderByPathQuery.Query(folder.Path));
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
                    await _exceptionService.LogErrorAsync(ex, "CreateFolderAndOneFileCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
