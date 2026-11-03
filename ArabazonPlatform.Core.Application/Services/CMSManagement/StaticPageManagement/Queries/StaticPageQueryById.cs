using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.CMS;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains.CMS;
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
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.CMSManagement.StaticPageManagement.Queries
{
    public class StaticPageQueryById
    {
        public class Query : IRequest<BaseResponse<StaticPageModel>>
        {
            public int StaticPageId { get; set; }
            public Query(int staticPageId)
            {
                StaticPageId = staticPageId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<StaticPageModel>>
        {
            public async Task<BaseResponse<StaticPageModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    StaticPageModel staticPage = _unitOfWork.GetRepository<StaticPage>().GetByExpressionAsNoTracking(p => p.StaticPageId == request.StaticPageId).Select(s => new StaticPageModel()
                    {
                        StaticPageId = s.StaticPageId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Content = s.Content,
                        ContentAr = s.ContentAr
                    }).FirstOrDefault();                    

                    return new BaseResponse<StaticPageModel>(true, string.Empty, string.Empty, staticPage);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "StaticPageQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<StaticPageModel>(false, ex.Message);
                }
            }
        }
    }
}
