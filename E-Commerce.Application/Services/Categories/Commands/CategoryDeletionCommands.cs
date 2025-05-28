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

namespace E_Commerce.Application.Services.Categories.Commands
{
    public class CategoryDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int CategoryId { get; set; }
            public Command(int categoryId)
            {
                this.CategoryId = categoryId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.CategoryId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }

                    Domain.Domains.Category category = await _unitOfWork.GetRepository<Domain.Domains.Category>().GetById(request.CategoryId);
                    _unitOfWork.GetRepository<Domain.Domains.Category>().Delete(category);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CategoryDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int categoryId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<Domain.Domains.Category>().GetByExpressionAsNoTracking(i => i.CategoryId == categoryId && (i.Items.Any() || i.ParentCategoryId == categoryId)).Any();
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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), actionId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
