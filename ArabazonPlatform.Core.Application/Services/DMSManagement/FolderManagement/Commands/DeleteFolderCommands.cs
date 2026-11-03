using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Services.DMSManagement.FileManagement.Commands;
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
    public class DeleteFolderCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int FolderId { get; set; }
            public bool IsCommit { get; set; }
            public Command(int folderId,bool isCommit = true)
            {
                this.FolderId = folderId;
                this.IsCommit = isCommit;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse responseDeleteFilesByFolderIdCommands = await _mediator.Send(new DeleteFilesByFolderIdCommands.Command(request.FolderId));
                    if (!responseDeleteFilesByFolderIdCommands.IsSuccess)
                    {
                        return responseDeleteFilesByFolderIdCommands;
                    }
                    IQueryable<Folder> folders = _unitOfWork.GetRepository<Folder>().GetByExpression(z => z.FolderId == request.FolderId);
                    Folder folder = await _unitOfWork.GetRepository<Folder>().FirstOrDefaultAsync(folders);

                    _unitOfWork.GetRepository<Folder>().Delete(folder);

                    if (request.IsCommit)
                    {
                        await _unitOfWork.CommitChanges();
                    }
                    await _mediator.Send(new DeletePhsiycalFolderCommands.Command(folder.Path));
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "DeleteFolderCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
