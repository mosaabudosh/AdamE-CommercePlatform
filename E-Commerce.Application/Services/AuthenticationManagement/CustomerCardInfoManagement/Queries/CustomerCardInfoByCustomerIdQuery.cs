using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.Authentication;
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

namespace E_Commerce.Application.Services.AuthenticationManagement.CustomerCardInfoManagement.Queries
{
    public class CustomerCardInfoByCustomerIdQuery
    {
        public class Query : IRequest<BaseResponse<List<CustomerCardInfoModel>>>
        {
            public Guid CustomerId { get; set; }
            public Query(Guid customerId)
            {
                CustomerId = customerId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<CustomerCardInfoModel>>>
        {
            public async Task<BaseResponse<List<CustomerCardInfoModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CustomerCardInfoModel> customerCardInfoModels = _unitOfWork.GetRepository<CustomerCardInfo>().GetByExpression(p => p.CustomerId == request.CustomerId).Select(s => new CustomerCardInfoModel
                    {
                        CustomerCardInfoId = s.CustomerCardInfoId,
                        IsRememberCard = s.IsRememberCard,
                        CardNumber = s.CardNumber,
                        Expiry = s.Expiry,
                        CustomerId = s.CustomerId
                    }).ToList();

                    return new BaseResponse<List<CustomerCardInfoModel>>(true, string.Empty, string.Empty, customerCardInfoModels);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CustomerCardInfoByCustomerIdQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<CustomerCardInfoModel>>(false, ex.Message);
                }
            }
        }
    }
}
