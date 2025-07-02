using Adam.Logging.Interfaces;
using Adam.Logging.Services.MSSQLServer;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Commands
{
    public class GenerateOTPCodeCommands
    {
        public class Command : IRequest<string>
        {
            public static Random random = new Random();
          
            public class Handler() : IRequestHandler<Command, string>
            {
                public async Task<string> Handle(Command request, CancellationToken cancellationToken)
                {
                    const string chars = "0123456789";
                    return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
                }

            }

        }

    }
}
