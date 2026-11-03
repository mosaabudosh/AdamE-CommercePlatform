using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.DashBoard;
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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.TaxsManagement.Queries
{
    public class TaxQueryById
    {
        public class Query : IRequest<BaseResponse<TaxModel>>
        {
            public int TaxId { get; set; }
            public Query(int taxId)
            {
                TaxId = taxId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<TaxModel>>
        {
            public async Task<BaseResponse<TaxModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    TaxModel tax = _unitOfWork.GetRepository<Tax>().GetByExpressionAsNoTracking(p => p.TaxId == request.TaxId).Select(s => new TaxModel()
                    {
                        TaxId = request.TaxId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Value = s.Value,
                        Code = s.Code,
                    }).FirstOrDefault();
                    return new BaseResponse<TaxModel>(true, string.Empty, string.Empty, tax);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "TaxQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<TaxModel>(false, ex.Message);
                }
            }
        }
    }
}
