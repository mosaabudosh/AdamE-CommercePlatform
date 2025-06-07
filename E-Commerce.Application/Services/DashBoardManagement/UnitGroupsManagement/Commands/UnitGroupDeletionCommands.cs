using Adam.Logging.Interfaces;
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

namespace E_Commerce.Application.Services.DashBoardManagement.UnitGroupsManagement.Commands
{
    public class UnitGroupDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int UnitGroupId { get; set; }
            public Command(int unitGroupId)
            {
                UnitGroupId = unitGroupId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.UnitGroupId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    UnitGroup unitGroup = await _unitOfWork.GetRepository<UnitGroup>().GetById(request.UnitGroupId);
                    _unitOfWork.GetRepository<UnitGroup>().Delete(unitGroup);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UnitGroupDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int unitGroupId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<UnitGroup>().GetByExpressionAsNoTracking(i => i.UnitGroupId == unitGroupId && i.ShopItems.Any()).Any();
                    if (haveRelatedData)
                    {
                        return new BaseResponse(false, string.Empty, ErrorCode.RelatedData);
                    }
                    return new BaseResponse(true, string.Empty);

                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), unitGroupId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
