using Adam.Logging.Interfaces;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands
{
    public class DeletePhsiycalFolderCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public string Path { get; set; }
            public Command(string path)
            {
                this.Path = path;
            }
        }

        public class Handler( IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(request.Path);
                    directoryInfo.Delete(true);

                    return new BaseResponse(true);
                }
                catch (System.IO.IOException ioExp)
                {
                    await _exceptionService.LogErrorAsync(ioExp, "DeletePhsiycalFolderCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse(false, ioExp.Message);
                }
            }

        }
    }
}
