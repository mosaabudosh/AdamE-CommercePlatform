using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;


namespace ArabazonPlatform.Application.Services.Language.Queries
{
    public class LanguagesDDLQuery
    {
        public record Query() : IRequest<BaseResponse<List<DDLDataSource>>> { }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<DDLDataSource>>>
        {
            public async Task<BaseResponse<List<DDLDataSource>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<DDLDataSource> languages = _unitOfWork.GetRepository<Domain.Domains.DashBoard.Language>().GetByExpressionAsNoTracking().Select(p => new DDLDataSource()
                    {
                        Value = p.LanguageId,
                        Label = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? p.Name : p.NameAr,
                    }).ToList();
                    return new BaseResponse<List<DDLDataSource>>(true, string.Empty, string.Empty, languages);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "LanguagesDDLQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
                }
            }
        }
    }
}
