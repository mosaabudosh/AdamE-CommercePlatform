using Adam.Logging.Interfaces;
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
    public class DiscountDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int DiscountId { get; set; }
            public Command(int discountId)
            {
                DiscountId = discountId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Discount discount = await _unitOfWork.GetRepository<Domain.Domains.Discount>().GetById(request.DiscountId);

                    discount.IsDeleted = true;
                    discount.DeletedDate = DateTime.UtcNow;
                    discount.DeletedUserId = _userAccessor.GetCurrentUserId();

                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "DiscountDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
