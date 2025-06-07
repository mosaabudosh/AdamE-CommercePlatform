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
    public class UnitUpdationCommands
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
                    Unit exisitUnit = await _unitOfWork.GetRepository<Unit>().GetById(request.UnitModel.UnitId);

                    exisitUnit.Name = request.UnitModel.Name.Trim();
                    exisitUnit.NameAr = !string.IsNullOrEmpty(request.UnitModel.NameAr) ? request.UnitModel.NameAr.Trim() : request.UnitModel.Name.Trim();
                    exisitUnit.Value = request.UnitModel.Value;
                    exisitUnit.UnitGroupId = request.UnitModel.UnitGroupId;

                    exisitUnit.LastUpdatedDate = DateTime.UtcNow;
                    exisitUnit.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitUnit);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }

                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UnitUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Unit unit)
            {
                try
                {
                    Unit existingRecord = _unitOfWork.GetRepository<Unit>().GetByExpressionAsNoTracking(x => (x.Name == unit.Name || x.NameAr == unit.NameAr) && x.UnitId != unit.UnitId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), unit);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
