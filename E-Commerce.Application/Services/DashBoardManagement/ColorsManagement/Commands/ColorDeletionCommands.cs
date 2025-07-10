using Adam.Logging.Interfaces;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.Generic.Constants.Constants;
using Color = E_Commerce.Domain.Domains.DashBoard.Color;

namespace E_Commerce.Application.Services.DashBoardManagement.ColorsManagement.Commands
{
    public class ColorDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int ColorId { get; set; }
            public Command(int colorId)
            {
                ColorId = colorId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.ColorId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    Color color = await _unitOfWork.GetRepository<Color>().GetById(request.ColorId);
                    _unitOfWork.GetRepository<Color>().Delete(color);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ColorDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int colorId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<Color>().GetByExpressionAsNoTracking(i => i.ColorId == colorId && i.OrderDetails.Any()).Any();
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), colorId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
