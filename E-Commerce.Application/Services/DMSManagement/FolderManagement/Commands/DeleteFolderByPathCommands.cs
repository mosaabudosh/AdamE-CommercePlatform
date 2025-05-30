using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using Adam.Logging.Services.MSSQLServer;
using E_Commerce.Application.Services.DMSManagement.FileManagement.Commands;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
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
using File = E_Commerce.Domain.Domains.DMS.File;

namespace E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands
{
    public class DeleteFolderByPathCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public string FolderPath { get; set; }
            public bool IsCommit { get; set; }

            public Command(string folderPath, bool isCommit = true)
            {
                FolderPath = folderPath;
                IsCommit = isCommit;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse<List<FileCreationModel>> responseFilesByFolderPathQuery = await _mediator.Send(new FilesByFolderPathQuery.Query(request.FolderPath));
                    if (!responseFilesByFolderPathQuery.IsSuccess)
                    {
                        return responseFilesByFolderPathQuery;
                    }
                    if (responseFilesByFolderPathQuery.DataObject.Any())
                    {
                        int folderId = responseFilesByFolderPathQuery.DataObject.FirstOrDefault(p => p.FolderId.HasValue).FolderId.Value;
                        IQueryable<File> foldersQuery = _unitOfWork.GetRepository<File>().GetByExpression(z => z.FolderId == folderId);
                        List<File> files = await _unitOfWork.GetRepository<File>().ToListAsync(foldersQuery);

                        _unitOfWork.GetRepository<File>().RemoveRange(files);

                        BaseResponse responseDeleteFolderCommands = await _mediator.Send(new DeleteFolderCommands.Command(folderId,request.IsCommit));
                        if (!responseDeleteFolderCommands.IsSuccess)
                        {
                            return responseDeleteFolderCommands;
                        }
                    }
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "DeleteFolderByPathCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
