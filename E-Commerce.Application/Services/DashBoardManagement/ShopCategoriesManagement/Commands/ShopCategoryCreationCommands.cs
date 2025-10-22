using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
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
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.ShopCategoriesManagement.Commands
{
    public class ShopCategoryCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ShopCategoryModel ShopCategoryModel { get; set; }
            public Command(ShopCategoryModel shopCategoryModel)
            {
                ShopCategoryModel = shopCategoryModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    ShopCategory shopCategory = new ShopCategory()
                    {
                        CategoryId = request.ShopCategoryModel.CategoryId,
                        ShopId = request.ShopCategoryModel.ShopId,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(shopCategory);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<ShopCategory>().Insert(shopCategory);
                    await _unitOfWork.CommitChanges();

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopCategoryCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(ShopCategory shopCategory)
            {
                try
                {
                    ShopCategory existingRecord = _unitOfWork.GetRepository<ShopCategory>().GetByExpressionAsNoTracking(x => x.ShopId == shopCategory.ShopId && x.CategoryId == shopCategory.CategoryId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), shopCategory);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
