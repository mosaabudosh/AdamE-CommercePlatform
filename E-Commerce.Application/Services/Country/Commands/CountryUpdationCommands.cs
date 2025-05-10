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
    public class CountryUpdationCommands
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
                    Domain.Domains.Country country = await _unitOfWork.GetRepository<Domain.Domains.Country>().GetById(request.CountryModel.CountryId);
                    country.Name = request.CountryModel.Name.Trim();
                    country.NameAr = request.CountryModel.NameAr.Trim();
                    country.LastUpdatedDate = DateTime.UtcNow;
                    country.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                    await _unitOfWork.CommitChanges();

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CountryUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
