using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.Authorisation;
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

namespace E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Queries
{
    public class GetUsersByShopItemIdQuery
    {
        public class Query : IRequest<BaseResponse<List<Guid>>>
        {
            public Guid ShopItemId { get; set; }
            public Query(Guid shopItemId)
            {
                ShopItemId = shopItemId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<Guid>>>
        {
            public async Task<BaseResponse<List<Guid>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<Guid> userIds = _unitOfWork.GetRepository<User>().GetByExpression(p => p.Shop.ShopItems.Any(s => s.ShopItemId == request.ShopItemId)).Select(s =>  
                         s.UserId
                    ).ToList();

                    return new BaseResponse<List<Guid>>(true, string.Empty, string.Empty, userIds);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "GetUsersByShopItemIdQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<Guid>>(false, ex.Message);
                }
            }
        }
    }
}
