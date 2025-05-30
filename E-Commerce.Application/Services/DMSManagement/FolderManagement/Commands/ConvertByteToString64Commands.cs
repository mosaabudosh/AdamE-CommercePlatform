using Adam.Logging.Interfaces;
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
    public class ConvertByteToString64Commands
    {
        public class Command : IRequest<string>
        {
            public string Extension { get; set; }
            public byte[] File { get; set; }

            public Command(byte[] file, string extension)
            {
                this.File = file;
                this.Extension = extension;

            }
        }

        public class Handler() : IRequestHandler<Command, string>
        {
            public async Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.File.Any())
                {
                    if (request.Extension == "pdf")
                    {
                        return $"data:application/{request.Extension};base64,{Convert.ToBase64String(request.File)}";
                    }
                    return $"data:image/{request.Extension};base64,{Convert.ToBase64String(request.File)}";
                }
                return null;
            }
        }
    }
}
