using Adam.Logging.Interfaces;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Generic.Constants.Constants;
using Unit = ArabazonPlatform.Domain.Domains.DashBoard.Unit;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.UnitsManagement.Commands
{
    public class UnitDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int UnitId { get; set; }
            public Command(int unitId)
            {
                UnitId = unitId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.UnitId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    Unit unit = await _unitOfWork.GetRepository<Unit>().GetById(request.UnitId);
                    _unitOfWork.GetRepository<Unit>().Delete(unit);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UnitDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int unitId)
            {
                try
                {
                    //bool haveRelatedData = _unitOfWork.GetRepository<Unit>().GetByExpressionAsNoTracking(i => i.UnitId == unitId && i.UnitGroup.Any()).Any();
                    //if (haveRelatedData)
                    //{
                    //    return new BaseResponse(false, string.Empty, ErrorCode.RelatedData);
                    //}
                    return new BaseResponse(true, string.Empty);

                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), unitId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
