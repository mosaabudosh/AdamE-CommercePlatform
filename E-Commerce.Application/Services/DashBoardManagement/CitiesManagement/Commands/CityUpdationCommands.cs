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

namespace E_Commerce.Application.Services.DashBoardManagement.CitiesManagement.Commands
{
    public class CityUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CityModel CityModel { get; set; }
            public Command(CityModel cityModel)
            {
                CityModel = cityModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.City city = await _unitOfWork.GetRepository<Domain.Domains.City>().GetById(request.CityModel.CityId);
                    city.Name = request.CityModel.Name.Trim();
                    city.NameAr = request.CityModel.NameAr.Trim();
                    city.CountryId = request.CityModel.CountryId;
                    city.LastUpdatedDate = DateTime.UtcNow;
                    city.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    await _unitOfWork.CommitChanges();

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CityUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
