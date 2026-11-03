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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.DiscountManagement.Commands
{
    public class DiscountUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public DiscountModel DiscountModel { get; set; }
            public Command(DiscountModel discountModel)
            {
                DiscountModel = discountModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Discount exisitDiscount = await _unitOfWork.GetRepository<Discount>().GetById(request.DiscountModel.DiscountId);

                    exisitDiscount.Name = request.DiscountModel.Name.Trim();
                    exisitDiscount.NameAr = request.DiscountModel.NameAr.Trim();
                    exisitDiscount.Code = request.DiscountModel.Code.Trim();
                    exisitDiscount.Description = request.DiscountModel.Description?.Trim();
                    exisitDiscount.EndDate = request.DiscountModel.EndDate;
                    exisitDiscount.FirstValueUse = request.DiscountModel.FirstValueUse;
                    exisitDiscount.StartDate = request.DiscountModel.StartDate;
                    exisitDiscount.Value = request.DiscountModel.Value;
                    exisitDiscount.MaxNumberOfUsage = request.DiscountModel.MaxNumberOfUsage;
                    exisitDiscount.LastUpdatedDate = DateTime.UtcNow;
                    exisitDiscount.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitDiscount);
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
                    await _exceptionService.LogErrorAsync(ex, "DiscountUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Discount discount)
            {
                try
                {
                    Discount existingRecord = _unitOfWork.GetRepository<Discount>().GetByExpressionAsNoTracking(x => (x.Name == discount.Name || x.NameAr == discount.NameAr || x.Code == discount.Code) && x.DiscountId != discount.DiscountId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), discount);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
