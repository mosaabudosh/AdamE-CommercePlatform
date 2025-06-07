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

namespace E_Commerce.Application.Services.DashBoardManagement.TaxsManagement.Commands
{
    public class TaxCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public TaxModel TaxModel { get; set; }
            public Command(TaxModel taxModel)
            {
                TaxModel = taxModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Tax tax = new Tax()
                    {
                        Name = request.TaxModel.Name.Trim(),
                        NameAr = !string.IsNullOrEmpty(request.TaxModel.NameAr) ? request.TaxModel.NameAr.Trim() : request.TaxModel.Name.Trim(),
                        Value = request.TaxModel.Value,
                        Code = request.TaxModel.Code,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(tax);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Tax>().Insert(tax);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "TaxCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Tax tax)
            {
                try
                {
                    Tax existingRecord = _unitOfWork.GetRepository<Tax>().GetByExpressionAsNoTracking(x => x.Name == tax.Name || x.NameAr == tax.NameAr).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), tax);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
