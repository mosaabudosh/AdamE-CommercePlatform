using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
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
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ShopCategoriesManagement.Commands
{
    public class ShopCategoryCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public List<ShopCategoryModel> ShopCategoryModels { get; set; }
            public Command(List<ShopCategoryModel> shopCategoryModels)
            {
                ShopCategoryModels = shopCategoryModels;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ShopCategory> newShopCategories = new List<ShopCategory>();
                    if(request.ShopCategoryModels.Any(p => p.ShopCategoryId == 0))
                    {
                        newShopCategories = request.ShopCategoryModels.Where(p => p.ShopCategoryId == 0).Select(s => new ShopCategory()
                        {
                            CategoryId = s.CategoryId,
                            ShopId = s.ShopId,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        }).ToList();

                        _unitOfWork.GetRepository<ShopCategory>().AddRange(newShopCategories);
                        await _unitOfWork.CommitChanges();
                    }

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
           
        }
    }
}
