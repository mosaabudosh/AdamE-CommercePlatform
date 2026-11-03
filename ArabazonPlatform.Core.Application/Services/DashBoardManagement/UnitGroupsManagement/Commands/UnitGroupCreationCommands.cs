using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.DashBoard;
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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.UnitGroupsManagement.Commands
{
    public class UnitGroupCreationCommands
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
                    UnitGroup unitGroup = new UnitGroup()
                    {
                        Name = request.UnitGroupModel.Name.Trim(),
                        NameAr = !string.IsNullOrEmpty(request.UnitGroupModel.NameAr) ? request.UnitGroupModel.NameAr.Trim() : request.UnitGroupModel.Name.Trim(),                        
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(unitGroup);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<UnitGroup>().Insert(unitGroup);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UnitGroupCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(UnitGroup unitGroup)
            {
                try
                {
                    UnitGroup existingRecord = _unitOfWork.GetRepository<UnitGroup>().GetByExpressionAsNoTracking(x => x.Name == unitGroup.Name || x.NameAr == unitGroup.NameAr).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), unitGroup);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
