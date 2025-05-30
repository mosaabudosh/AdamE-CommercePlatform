using Adam.Logging.Interfaces;
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

namespace E_Commerce.Application.Services.DMSManagement.FileManagement.Commands
{
    public class DeleteFilesByFolderIdCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int FolderId { get; set; }
            public Command(int folderId)
            {
                this.FolderId = folderId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    IQueryable<File> foldersQuery = _unitOfWork.GetRepository<File>().GetByExpression(z => z.FolderId == request.FolderId);
                    List<File> files = await _unitOfWork.GetRepository<File>().ToListAsync(foldersQuery);
                    if (files.Any())
                    {
                        _unitOfWork.GetRepository<File>().RemoveRange(files);
                    }
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "DeleteFilesByFolderIdCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
