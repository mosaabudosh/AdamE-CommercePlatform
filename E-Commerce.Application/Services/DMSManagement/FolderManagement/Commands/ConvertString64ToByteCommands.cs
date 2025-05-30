using Adam.Logging.Interfaces;
using E_Commerce.Domain.Domains.DMS;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands
{
    public class ConvertString64ToByteCommands
    {
        public class Command : IRequest<byte[]>
        {
            public string Base64String { get; set; }
            public Command(string base64String)
            {
                this.Base64String = base64String;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, byte[]>
        {
            public async Task<byte[]> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!string.IsNullOrEmpty(request.Base64String))
                    {
                        if (request.Base64String.Contains(","))
                        {
                            request.Base64String = request.Base64String.Substring(request.Base64String.IndexOf(",") + 1);
                        }
                        return Convert.FromBase64String(request.Base64String);
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ConvertString64ToByteCommands", _userAccessor.GetCurrentUserId(), request);
                    return null;
                }
            }
        }

    }
}
