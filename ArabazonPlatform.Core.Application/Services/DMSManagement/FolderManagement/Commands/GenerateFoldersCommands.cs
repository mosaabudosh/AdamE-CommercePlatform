using Adam.Logging.Interfaces;
using ArabazonPlatform.Domain.Domains;
using ArabazonPlatform.Domain.Domains.DMS;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands
{
    public class GenerateFoldersCommands
    {
        public class Command : IRequest<BaseResponse<List<Folder>>>
        {
            public List<Folder> Folders { get; set; }
            public List<Folder> NewFolders { get; set; }
            public Folder Folder { get; set; }
            public string[] Paths { get; set; }
            public string PathFromFolderConfiguration { get; set; }

            public Command(List<Folder> folders, List<Folder> newFolders, Folder folder, string[] paths, string pathFromFolderConfiguration)
            {
                this.Folders = folders;
                this.NewFolders = newFolders;
                this.Folder = folder;
                this.Paths = paths;
                this.PathFromFolderConfiguration = pathFromFolderConfiguration;
            }

            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse<List<Folder>>>
            {
                public async Task<BaseResponse<List<Folder>>> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        if (request.Paths.Any())
                        {
                            int i = 0;
                            foreach (string newPath in request.Paths)
                            {
                                if (i <= request.Paths.Length - 2)
                                {
                                    Folder newFolder = new Folder();
                                    newFolder.Path = (i == 0) ? request.PathFromFolderConfiguration + newPath : request.Folders[i - 1].Path + "\\" + newPath;
                                    newFolder.CreationUserId = _userAccessor.GetCurrentUserId();
                                    newFolder.IsActive = true;
                                    newFolder.Name = newPath;
                                    newFolder.CreationDate = DateTime.Now;
                                    newFolder.ParentFolderId = (i > 0) ? request.Folders[i - 1].FolderId : newFolder.ParentFolderId;
                                    newFolder.ParentFolder = (i > 0) ? request.Folders[i - 1] : null;
                                    Folder existFolder = _unitOfWork.GetRepository<Folder>().GetByExpression(x => x.Path.Contains(newFolder.Path)).FirstOrDefault();
                                    if (existFolder == null)
                                    {
                                        request.NewFolders.Add(newFolder);
                                        request.Folders.Add(newFolder);
                                    }
                                    else
                                    {
                                        request.Folders.Add(existFolder);
                                    }
                                }
                                i++;
                            }
                        }
                        request.Folder.ParentFolder = request.Folders.LastOrDefault();
                        request.Folder.ParentFolderId = request.Folders.LastOrDefault().FolderId;
                        request.Folder.Path = $"{request.PathFromFolderConfiguration}{request.Folder.Path}\\{request.Folder.Name}";
                        request.NewFolders.Add(request.Folder);
                        return new BaseResponse<List<Folder>>(true, string.Empty, request.NewFolders);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "GenerateFoldersCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse<List<Folder>>(true, ex.Message);
                    }
                }
            }
        }
    }
}
