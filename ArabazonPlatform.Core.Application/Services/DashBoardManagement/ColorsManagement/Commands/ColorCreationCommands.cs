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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ColorsManagement.Commands
{
    public class ColorCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ColorModel ColorModel { get; set; }
            public Command(ColorModel colorGroup)
            {
                ColorModel = colorGroup;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Color color = new Color()
                    {
                        Name = request.ColorModel.Name.Trim(),
                        NameAr = !string.IsNullOrEmpty(request.ColorModel.NameAr) ? request.ColorModel.NameAr.Trim() : request.ColorModel.Name.Trim(),
                        Value = request.ColorModel.Value.Trim(),
                        ColorGroupId = request.ColorModel.ColorGroupId,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(color);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Color>().Insert(color);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ColorCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Color color)
            {
                try
                {
                    Color existingRecord = _unitOfWork.GetRepository<Color>().GetByExpressionAsNoTracking(x => x.Name == color.Name || x.NameAr == color.NameAr).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), color);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
