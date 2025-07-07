using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Domain.Domains.Authentication;
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

namespace E_Commerce.Application.Services.DashBoardManagement.CustomerFavoritesManagement.Commands
{
    public class CustomerFavoriteUpdationCommands
    {
        public class Command : IRequest<BaseResponse<CustomerFavoriteModel>>
        {
            public CustomerFavoriteModel CustomerFavoriteModel { get; set; }
            public Command(CustomerFavoriteModel customerFavoriteModel)
            {
                CustomerFavoriteModel = customerFavoriteModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse<CustomerFavoriteModel>>
        {
            public async Task<BaseResponse<CustomerFavoriteModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Guid customerId = request.CustomerFavoriteModel.CustomerId;
                    List<CustomerFavorite> exisitCustomerFavorites = _unitOfWork.GetRepository<CustomerFavorite>().GetByExpression(p => p.CustomerId == customerId).ToList();

                    List<CustomerFavorite> removeSavedCustomerFavorites = new List<CustomerFavorite>();
                    List<CustomerFavorite> newCustomerFavorite = new List<CustomerFavorite>();

                    if (!request.CustomerFavoriteModel.CustomerFavoriteDetailModels.Any())
                    {
                        _unitOfWork.GetRepository<CustomerFavorite>().RemoveRange(exisitCustomerFavorites);
                    }

                    List<Guid> stableCustomerFavoriteIds = request.CustomerFavoriteModel.CustomerFavoriteDetailModels.Where(p => p.CustomerFavoriteId.HasValue).Select(x => x.CustomerFavoriteId.Value).ToList();

                    if (stableCustomerFavoriteIds.Any())
                    {
                        removeSavedCustomerFavorites = _unitOfWork.GetRepository<CustomerFavorite>().GetByExpressionAsNoTracking(p => !stableCustomerFavoriteIds.Contains(p.CustomerFavoriteId) && p.CustomerId == customerId).ToList();
                        
                        _unitOfWork.GetRepository<CustomerFavorite>().RemoveRange(removeSavedCustomerFavorites);
                    }

                    newCustomerFavorite = request.CustomerFavoriteModel.CustomerFavoriteDetailModels.Where(p => !p.CustomerFavoriteId.HasValue).Select(s => new CustomerFavorite()
                    {
                        CustomerId = customerId,
                        Discount = s.Discount,
                        Price = s.Price,
                        Quantity = s.Quantity,
                        ShopItemId = s.ShopItemId,
                        Tax = s.Tax,
                        IsActive = true,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow
                    }).ToList();

                    if (newCustomerFavorite.Any())
                    {
                        _unitOfWork.GetRepository<CustomerFavorite>().AddRange(newCustomerFavorite);
                    }

                    await _unitOfWork.CommitChanges();

                    List<CustomerFavoriteDetailModel> customerFavoriteDetailModels = _unitOfWork.GetRepository<CustomerFavorite>().GetByExpressionAsNoTracking(p => p.CustomerId == request.CustomerFavoriteModel.CustomerId).Select(s => new CustomerFavoriteDetailModel()
                    {
                        CustomerFavoriteId = s.CustomerFavoriteId,
                        ShopItemId = s.ShopItemId,
                        Discount = s.Discount,
                        Price = s.Price,
                        Quantity = s.Quantity,
                        Tax = s.Tax,
                    }).ToList();

                    CustomerFavoriteModel customerFavoriteModel = new CustomerFavoriteModel()
                    {
                        CustomerId = request.CustomerFavoriteModel.CustomerId,
                        CustomerFavoriteDetailModels = customerFavoriteDetailModels
                    };

                    return new BaseResponse<CustomerFavoriteModel>(true, string.Empty, customerFavoriteModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CustomerFavoriteUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<CustomerFavoriteModel>(false, ex.Message);
                }
            }
        }
    }
}
