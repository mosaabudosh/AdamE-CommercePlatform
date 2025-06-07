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

namespace E_Commerce.Application.Services.DashBoardManagement.PaymentMethodsManagement.Queries
{
    public class PaymentMethodQueryById
    {
        public class Query : IRequest<BaseResponse<PaymentMethodModel>>
        {
            public int PaymentMethodId { get; set; }
            public Query(int paymentMethodId)
            {
                PaymentMethodId = paymentMethodId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<PaymentMethodModel>>
        {
            public async Task<BaseResponse<PaymentMethodModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    PaymentMethodModel paymentMethod = _unitOfWork.GetRepository<PaymentMethod>().GetByExpressionAsNoTracking(p => p.PaymentMethodId == request.PaymentMethodId).Select(s => new PaymentMethodModel()
                    {
                        PaymentMethodId = request.PaymentMethodId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Code = s.Code,
                        Icon =s.Icon,
                    }).FirstOrDefault();
                    return new BaseResponse<PaymentMethodModel>(true, string.Empty, string.Empty, paymentMethod);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "PaymentMethodQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<PaymentMethodModel>(false, ex.Message);
                }
            }
        }
    }
}
