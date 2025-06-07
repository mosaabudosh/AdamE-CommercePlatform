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

namespace E_Commerce.Application.Services.DashBoardManagement.UnitGroupsManagement.Commands
{
    public class UnitGroupUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public UnitGroupModel UnitGroupModel { get; set; }
            public Command(UnitGroupModel unitGroupModel)
            {
                UnitGroupModel = unitGroupModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    UnitGroup exisitUnitGroup = await _unitOfWork.GetRepository<UnitGroup>().GetById(request.UnitGroupModel.UnitGroupId);

                    exisitUnitGroup.Name = request.UnitGroupModel.Name.Trim();
                    exisitUnitGroup.NameAr = !string.IsNullOrEmpty(request.UnitGroupModel.NameAr) ? request.UnitGroupModel.NameAr.Trim() : request.UnitGroupModel.Name.Trim();
                    exisitUnitGroup.LastUpdatedDate = DateTime.UtcNow;
                    exisitUnitGroup.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitUnitGroup);
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
                    await _exceptionService.LogErrorAsync(ex, "UnitGroupUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(UnitGroup unitGroup)
            {
                try
                {
                    UnitGroup existingRecord = _unitOfWork.GetRepository<UnitGroup>().GetByExpressionAsNoTracking(x => (x.Name == unitGroup.Name || x.NameAr == unitGroup.NameAr) && x.UnitGroupId != unitGroup.UnitGroupId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), unitGroup);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
