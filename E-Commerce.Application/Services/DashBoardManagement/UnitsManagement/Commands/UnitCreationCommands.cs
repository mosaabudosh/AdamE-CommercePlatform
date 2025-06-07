using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.Generic.Constants.Constants;
using Unit = E_Commerce.Domain.Domains.DashBoard.Unit;

namespace E_Commerce.Application.Services.DashBoardManagement.UnitsManagement.Commands
{
    public class UnitCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public UnitModel UnitModel { get; set; }
            public Command(UnitModel unitModel)
            {
                UnitModel = unitModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Unit unit = new Unit()
                    {
                        Name = request.UnitModel.Name.Trim(),
                        NameAr = !string.IsNullOrEmpty(request.UnitModel.NameAr) ? request.UnitModel.NameAr.Trim() : request.UnitModel.Name.Trim(),
                        UnitGroupId = request.UnitModel.UnitGroupId,
                        Value = request.UnitModel.Value,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(unit);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Unit>().Insert(unit);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UnitCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Unit unit)
            {
                try
                {
                    Unit existingRecord = _unitOfWork.GetRepository<Unit>().GetByExpressionAsNoTracking(x => x.Name == unit.Name || x.NameAr == unit.NameAr).FirstOrDefault();

                    if (existingRecord is not null)
                    {
                        return new BaseResponse(false, string.Empty, ErrorCode.AlreadyExsist);
                    }
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), unit);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
