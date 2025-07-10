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

namespace E_Commerce.Application.Services.DashBoardManagement.ColorGroupsManagement.Commands
{
    public class ColorGroupUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ColorGroupModel ColorGroupModel { get; set; }
            public Command(ColorGroupModel colorGroupModel)
            {
                ColorGroupModel = colorGroupModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    ColorGroup exisitRecord = await _unitOfWork.GetRepository<ColorGroup>().GetById(request.ColorGroupModel.ColorGroupId.Value);

                    exisitRecord.Name = request.ColorGroupModel.Name.Trim();
                    exisitRecord.NameAr = !string.IsNullOrEmpty(request.ColorGroupModel.NameAr) ? request.ColorGroupModel.NameAr.Trim() : request.ColorGroupModel.Name.Trim();
                    exisitRecord.LastUpdatedDate = DateTime.UtcNow;
                    exisitRecord.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitRecord);
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
                    await _exceptionService.LogErrorAsync(ex, "ColorGroupUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(ColorGroup colorGroup)
            {
                try
                {
                    ColorGroup existingRecord = _unitOfWork.GetRepository<ColorGroup>().GetByExpressionAsNoTracking(x => (x.Name == colorGroup.Name || x.NameAr == colorGroup.NameAr) && x.ColorGroupId != colorGroup.ColorGroupId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), colorGroup);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
