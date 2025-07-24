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
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.ShopItemRatingsManagement.Commands
{
    public class ShopItemRatingCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ShopItemRatingModel ShopItemRatingModel { get; set; }
            public Command(ShopItemRatingModel shopItemRatingModel)
            {
                ShopItemRatingModel = shopItemRatingModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    ShopItemRating shopItemRating = new ShopItemRating()
                    {
                        Comment = request.ShopItemRatingModel.Comment,
                        CustomerId = request.ShopItemRatingModel.CustomerId,
                        Rating = request.ShopItemRatingModel.Rating,
                        ShopItemId = request.ShopItemRatingModel.ShopItemId,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };                 
                    _unitOfWork.GetRepository<ShopItemRating>().Insert(shopItemRating);
                    await _unitOfWork.CommitChanges();

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemRatingCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
