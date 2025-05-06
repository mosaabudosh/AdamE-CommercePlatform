

using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;

namespace E_Commerce.Application.Services.UserType.Queries
{
    public class UserTypeQuery
    {
        public record Query() : IRequest<BaseResponse<List<DDLDataSource>>>
        {
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<DDLDataSource>>>
        {
            public async Task<BaseResponse<List<DDLDataSource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DDLDataSource> userTypes = _unitOfWork.GetRepository<Domain.Domains.Authorisation.UserType>().GetByExpressionAsNoTracking().Select(p => new DDLDataSource()
                    {
                        Value = p.UserTypeId,
                        Label = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.Name : p.NameAr,
                    }).ToList();
                    return new BaseResponse<List<DDLDataSource>>(true, string.Empty, string.Empty, userTypes);
                }
                catch (Exception ex)
                {
                    await _exceptionService.LogErrorAsync(ex, "UserTypeQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
                }
            }
        }
    }
}
