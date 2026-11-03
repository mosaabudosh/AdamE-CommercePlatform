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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.DiscountManagement.Commands
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
                    Discount discount = await _unitOfWork.GetRepository<Discount>().GetById(request.DiscountId);

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
