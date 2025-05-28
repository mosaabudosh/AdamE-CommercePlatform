using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.Country.Commands
{
    public class CountryCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CountryModel CountryModel { get; set; }
            public Command(CountryModel countryModel)
            {
                this.CountryModel = countryModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Country country = new Domain.Domains.Country()
                    {
                        Name = request.CountryModel.Name.Trim(),
                        NameAr = request.CountryModel.NameAr.Trim(),
                        Banner = string.Empty,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    _unitOfWork.GetRepository<Domain.Domains.Country>().Insert(country);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CountryCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
