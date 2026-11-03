using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Domain.Domains.Authorisation;
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

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.CustomersManagement.Queries
{
    public class CustomersDDLQuery
    {
        public class Query : IRequest<BaseResponse<List<DDLDataSource>>> 
        {
            public string? SearchText { get; set; }
            public Query(string? searchText = null)
            {
                SearchText = searchText;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<DDLDataSource>>>
        {
            public async Task<BaseResponse<List<DDLDataSource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DDLDataSource> customers = _unitOfWork.GetRepository<Customer>().GetByExpressionAsNoTracking(p => p.IsActive && !p.IsBlock && (string.IsNullOrEmpty(request.SearchText) || (p.FirstName.Contains(request.SearchText) || p.SecondName.Contains(request.SearchText) || p.FirstNameAr.Contains(request.SearchText) || p.SecondNameAr.Contains(request.SearchText)))).Select(p => new DDLDataSource()
                    {
                        ValueGuid = p.CustomerId,
                        Label = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? $"{p.FirstName} {p.SecondName}" : $"{p.FirstNameAr} {p.SecondNameAr}",
                    }).ToList();
                    return new BaseResponse<List<DDLDataSource>>(true, string.Empty, string.Empty, customers);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CustomersDDLQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
                }
            }
        }
    }
}
