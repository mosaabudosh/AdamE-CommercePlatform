using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.AuthenticationManagement.ScreensManagement.Queries
{
    public class ScreensDDLQuery
    {
        public record Query() : IRequest<BaseResponse<List<DDLDataSource>>> { }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<DDLDataSource>>>
        {
            public async Task<BaseResponse<List<DDLDataSource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DDLDataSource> screens = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Screen>().GetByExpressionAsNoTracking(p => p.IsActive).Select(p => new DDLDataSource()
                    {
                        Value = p.ScreenId,
                        Label = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.Name : p.NameAr,
                    }).ToList();
                    return new BaseResponse<List<DDLDataSource>>(true, string.Empty, string.Empty, screens);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ScreensDDLQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
                }
            }
        }
    }
}
