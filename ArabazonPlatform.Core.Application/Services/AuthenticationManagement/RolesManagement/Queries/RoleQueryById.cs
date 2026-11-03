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
    public class RoleQueryById
    {
        public class Query : IRequest<BaseResponse<RoleModel>>
        {
            public int RoleId { get; set; }
            public Query(int roleId)
            {
                RoleId = roleId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<RoleModel>>
        {
            public async Task<BaseResponse<RoleModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    RoleModel role = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Role>().GetByExpressionAsNoTracking(p => p.RoleId == request.RoleId).Select(s => new RoleModel()
                    {
                        RoleId = s.RoleId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                    }).FirstOrDefault();
                    return new BaseResponse<RoleModel>(true, string.Empty, string.Empty, role);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "RoleQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<RoleModel>(false, ex.Message);
                }
            }
        }
    }
}
