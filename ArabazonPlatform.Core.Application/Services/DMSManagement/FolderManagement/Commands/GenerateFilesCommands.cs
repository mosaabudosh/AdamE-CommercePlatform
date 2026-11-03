using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Services.DMSManagement.FileType.Queries;
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
using File = ArabazonPlatform.Domain.Domains.DMS.File;


namespace ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands
{
    public class GenerateFilesCommands
    {
        public class Command : IRequest<BaseResponse<List<File>>>
        {
            public Folder Folder { get; set; }
            public List<FileCreationModel> FileCreationModels { get; set; }
            public string FolderPath { get; set; }

            public Command(Folder folder, List<FileCreationModel> fileCreationModels, string folderPath)
            {
                this.Folder = folder;
                this.FileCreationModels = fileCreationModels;
                this.FolderPath = folderPath;
            }
        }

        public class Handler(IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Command, BaseResponse<List<File>>>
        {
            public async Task<BaseResponse<List<File>>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    List<File> files = new List<File>();
                    foreach (FileCreationModel fileCreationModel in request.FileCreationModels)
                    {
                        BaseResponse createFolderAndFilesPhysically = await _mediator.Send(new CreateFolderAndFilesPhysicallyCommands.Command(request.Folder, fileCreationModel));
                        if (createFolderAndFilesPhysically.IsSuccess)
                        {
                            BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType> fileType = await _mediator.Send(new FileTypeByFileExtensionQuery.Query(fileCreationModel.FileType));
                            if (!fileType.IsSuccess)
                            {
                                return new BaseResponse<List<File>>(false, fileType.Message, fileType.ErrorCode);
                            }
                            byte[] fileContentBytes = await _mediator.Send(new ConvertString64ToByteCommands.Command(fileCreationModel.FileContent));
                            File file = new File()
                            {
                                IsActive = true,
                                CreationDate = DateTime.UtcNow,
                                CreationUserId = _userAccessor.GetCurrentUserId(),
                                Name = fileCreationModel.FileName,
                                FileTypeId = fileType.DataObject.FileTypeId,
                                Path = request.FolderPath,
                                Size = fileContentBytes.Length,
                            };

                            if (fileCreationModel.FileContent.Length < 1024)
                            {
                                file.SizeType = "byte";
                            }
                            if (fileCreationModel.FileContent.Length == 1024)
                            {
                                file.SizeType = "KB";
                            }
                            if (fileCreationModel.FileContent.Length > (1024 * 1024))
                            {
                                file.SizeType = "MB";
                            }
                            if (fileCreationModel.FileContent.Length > (1024 * 1024 * 1024))
                            {
                                file.SizeType = "GB";
                            }
                            files.Add(file);
                        }
                        else
                        {
                            return new BaseResponse<List<File>>(false, createFolderAndFilesPhysically.Message);
                        }
                    }
                    return new BaseResponse<List<File>>(true, string.Empty, files);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "GenerateFilesCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<File>>(true, ex.Message);
                }
            }
        }
    }
}
