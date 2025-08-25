using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DMS;
using E_Commerce.Application.Services.DMSManagement.FileType.Queries;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Domain.Domains.DMS;
using E_Commerce.Domain.Enum;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries
{
    public class FilesByFolderPathQuery
    {
        public class Query : IRequest<BaseResponse<List<FileCreationModel>>>
        {
            public string FolderPath { get; set; }
            public Query(string folderPath)
            {
                FolderPath = folderPath;
            }
        }
        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Query, BaseResponse<List<FileCreationModel>>>
        {
            public async Task<BaseResponse<List<FileCreationModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    IQueryable<Folder> foldersQuery = _unitOfWork.GetRepository<Folder>().GetByExpression(x => x.Path.Contains(request.FolderPath),
                    includedEntities: new List<System.Linq.Expressions.Expression<Func<Folder, object>>>() { z => z.Files });
                    Folder folder = await _unitOfWork.GetRepository<Folder>().FirstOrDefaultAsync(foldersQuery);
                    List<FileCreationModel> files = new List<FileCreationModel>();
                    if (folder is not null && folder.Files.Any())
                    {
                        List<int> fileTypeIds = folder.Files.Select(s => { return s.FileTypeId; }).ToList();

                        BaseResponse<List<E_Commerce.Domain.Domains.DMS.FileType>> fileType = await _mediator.Send(new FileTypeByIdsQuery.Query(fileTypeIds));
                        if (!fileType.IsSuccess)
                        {
                            await _exceptionService.LogErrorAsync(fileType.Message, "FilesByFolderPathCommands", _userAccessor.GetCurrentUserId(), fileTypeIds);
                            throw new Exception(fileType.Message);
                        }
                        List<FileTypeModel> fileTypes = fileType.DataObject.Select(s => new FileTypeModel()
                        {
                            FileTypeId = s.FileTypeId,
                            Extension = s.Extension,
                        }).ToList();

                        folder.Files.ForEach(async file =>
                        {
                            string fileType = fileTypes.FirstOrDefault(p => p.FileTypeId == file.FileTypeId).Extension;
                            byte[] fileInByte = System.IO.File.Exists($"{file.Path}\\{file.Name}.{fileType}") ? System.IO.File.ReadAllBytes($"{file.Path}\\{file.Name}.{fileType}") : [];
                            files.Add(new FileCreationModel()
                            {
                                FolderId = folder.FolderId,
                                FileType = fileType,
                                FileName = file.Name,
                                FolderName = folder.Name,
                                FileTypeId = file.FileTypeId,
                                FilePath = file.Path,
                                File = string.Empty,
                                FileContent = await _mediator.Send(new ConvertByteToString64Commands.Command(fileInByte, (file.FileTypeId == (int)Enums.FileTypes.SVG ? "svg+xml" : fileType)))
                            });
                        });
                    }
                    return new BaseResponse<List<FileCreationModel>>(true, string.Empty, files);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "FilesByFolderPathCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<FileCreationModel>>(false, ex.Message);
                }
            }

        }
    }
}
