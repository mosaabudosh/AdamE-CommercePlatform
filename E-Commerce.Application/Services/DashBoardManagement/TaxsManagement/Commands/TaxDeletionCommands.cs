using Adam.Logging.Interfaces;
using E_Commerce.Domain.Domains;
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

namespace E_Commerce.Application.Services.DashBoardManagement.TaxsManagement.Commands
{
    public class TaxDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int TaxId { get; set; }
            public Command(int taxId)
            {
                TaxId = taxId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.TaxId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    Tax tax = await _unitOfWork.GetRepository<Tax>().GetById(request.TaxId);
                    _unitOfWork.GetRepository<Tax>().Delete(tax);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "TaxDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int taxId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<Tax>().GetByExpressionAsNoTracking(i => i.TaxId == taxId && i.ShopItems.Any()).Any();
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), taxId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
