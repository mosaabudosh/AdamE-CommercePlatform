using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.RolesManagement.Queries
{
    public class RolesQuery
    {
        public class Query : IRequest<BaseResponse<List<RoleModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }


            public Query(int pageNumber, int pageSize, string? searchText = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<RoleModel>>>
        {
            public async Task<BaseResponse<List<RoleModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<RoleModel> roles = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Role>().GetByExpressionAsNoTracking().Select(s => new RoleModel()
                    {
                        RoleId = s.RoleId,
                        Name = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<RoleModel>>(true, string.Empty, string.Empty, roles);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "RolesQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<RoleModel>>(false, ex.Message);
                }
            }
        }
    }
}
