using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Domain.Domains.Authentication;
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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.CustomerFavoritesManagement.Commands
{
    public class CustomerFavoriteCreationCommands
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
                    if (exisitCustomerFavorites.Any())
                    {
                        _unitOfWork.GetRepository<CustomerFavorite>().RemoveRange(exisitCustomerFavorites);
                    }

                    List<CustomerFavorite> customerFavorites = request.CustomerFavoriteModel.CustomerFavoriteDetailModels.Select(s => new CustomerFavorite()
                    {
                        CustomerId = customerId,
                        ShopItemId = s.ShopItemId,
                        Discount = s.Discount,
                        Price = s.Price,
                        Quantity = s.Quantity,
                        Tax = s.Tax,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true,
                    }).ToList();

                    _unitOfWork.GetRepository<CustomerFavorite>().AddRange(customerFavorites);
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
                    await _exceptionService.LogErrorAsync(ex, "CustomerFavoriteCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<CustomerFavoriteModel>(false, ex.Message);
                }
            }
        }


    }
}
