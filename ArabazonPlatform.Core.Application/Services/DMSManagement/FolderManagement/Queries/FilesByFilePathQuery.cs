using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.DMS;
using ArabazonPlatform.Application.Services.DMSManagement.FileType.Queries;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Domains.DMS;
using ArabazonPlatform.Domain.Enum;
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

namespace ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries
{
    public class FilesByFilePathQuery
    {
        public class Query : IRequest<BaseResponse<List<FileCreationModel>>>
        {
            public List<string> FilePaths { get; set; }
            public bool? IncludeConfiguration { get; set; }

            public Query(List<string> filePaths , bool? includeConfiguration = true)
            {
                FilePaths = filePaths;
                IncludeConfiguration = includeConfiguration;
            }
        }
        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<FileCreationModel>>>
        {
            public async Task<BaseResponse<List<FileCreationModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    string pathFromFolderConfiguration = _cachingPlatformSettings[PlatformSettingsCode.DMSRootFolderPath];
                    if (request.IncludeConfiguration.HasValue && request.IncludeConfiguration.Value) 
                    {
                        request.FilePaths = request.FilePaths.Select(s => { return $"{pathFromFolderConfiguration}{s}"; }).ToList();
                    }
                   
                    IQueryable<File> filesQuery = _unitOfWork.GetRepository<File>().GetByExpression(x => request.FilePaths.Any(p => p == x.Path),
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
                            string filePath = $"{file.Path}\\{file.Name}.{file.FileType.Extension}";
                            byte[] fileInByte = System.IO.File.Exists(filePath) ? System.IO.File.ReadAllBytes(filePath) : [];
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
