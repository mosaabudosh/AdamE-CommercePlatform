using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
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
using static E_Commerce.Domain.Enum.Enums;
using File = E_Commerce.Domain.Domains.DMS.File;

namespace E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries
{
    public class FilesByFilePathQuery
    {
        public class Query : IRequest<BaseResponse<List<FileCreationModel>>>
        {
            public List<string> FilePaths { get; set; }
            public Query(List<string> filePaths)
            {
                FilePaths = filePaths;
            }
        }
        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<FileCreationModel>>>
        {
            public async Task<BaseResponse<List<FileCreationModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    string pathFromFolderConfiguration = _cachingPlatformSettings[PlatformSettingsCode.DMSRootFolderPath];

                    request.FilePaths = request.FilePaths.Select(s => { return $"{pathFromFolderConfiguration}{s}"; }).ToList();
                    IQueryable<File> filesQuery = _unitOfWork.GetRepository<File>().GetByExpression(x => request.FilePaths.Contains(x.Path),
                    includedEntities: new List<System.Linq.Expressions.Expression<Func<File, object>>>()
                    {
                        z => z.Folder,
                        z => z.FileType
                    });
                    List<File> files = await _unitOfWork.GetRepository<File>().ToListAsync(filesQuery);
                    List<FileCreationModel> images = new List<FileCreationModel>();
                    if (files.Any())
                    {
                        files.ForEach(async file =>
                        {
                            byte[] fileInByte = System.IO.File.ReadAllBytes($"{file.Path}\\{file.Name}.{file.FileType.Extension}");
                            int startIndex = file.Path.IndexOf(pathFromFolderConfiguration) + pathFromFolderConfiguration.Length;
                            int pathLength = file.Path.Length;
                            int length = pathLength - startIndex;

                            images.Add(new FileCreationModel()
                            {
                                FileType = file.FileType.Extension,
                                FileName = file.Name,
                                FolderName = file.Folder.Name,
                                FilePath = file.Path.Substring(startIndex, length),
                                FileTypeId = file.FileTypeId,
                                File = string.Empty,
                                FileContent = await _mediator.Send(new ConvertByteToString64Commands.Command(fileInByte, (file.FileTypeId == (int)Enums.FileTypes.SVG ? "svg+xml" : file.FileType.Extension)))
                            });
                        });
                    }
                    return new BaseResponse<List<FileCreationModel>>(true, string.Empty, images);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "FilesByFilePathQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<FileCreationModel>>(false, string.Empty);
                }
            }

        }
    }
}
