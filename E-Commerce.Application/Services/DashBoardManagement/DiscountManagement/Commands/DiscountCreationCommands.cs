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

namespace E_Commerce.Application.Services.DashBoardManagement.DiscountManagement.Commands
{
    public class DiscountCreationCommands
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
                    Discount discount = new Discount()
                    {
                        DiscountId = 0,
                        Name = request.DiscountModel.Name.Trim(),
                        NameAr = !string.IsNullOrEmpty(request.DiscountModel.NameAr) ? request.DiscountModel.NameAr.Trim() : request.DiscountModel.Name.Trim(),
                        Code = request.DiscountModel.Code.Trim(),
                        Description = request.DiscountModel.Description?.Trim(),
                        EndDate = request.DiscountModel.EndDate,
                        FirstValueUse = request.DiscountModel.FirstValueUse,
                        StartDate = request.DiscountModel.StartDate,
                        Value = request.DiscountModel.Value,
                        MaxNumberOfUsage = request.DiscountModel.MaxNumberOfUsage,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(discount);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Discount>().Insert(discount);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "DiscountCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Discount discount)
            {
                try
                {
                    Discount existingRecord = _unitOfWork.GetRepository<Discount>().GetByExpressionAsNoTracking(x => x.Name == discount.Name || x.NameAr == discount.NameAr || x.Code == discount.Code).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), discount);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
