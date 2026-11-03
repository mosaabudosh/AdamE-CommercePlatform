using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Domains.Authorisation;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.CustomersManagement.Commands
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
