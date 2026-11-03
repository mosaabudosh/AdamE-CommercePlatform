using Adam.Logging.Interfaces;
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

namespace ArabazonPlatform.Application.Services.DMSManagement.FileType.Queries
{
    public class FileTypeByIdsQuery
    {
        public class Query : IRequest<BaseResponse<List<ArabazonPlatform.Domain.Domains.DMS.FileType>>>
        {
            public List<int> FileTypeIds { get; set; }
            public Query(List<int> fileTypeIds)
            {
                this.FileTypeIds = fileTypeIds;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ArabazonPlatform.Domain.Domains.DMS.FileType>>>
        {
            public async Task<BaseResponse<List<ArabazonPlatform.Domain.Domains.DMS.FileType>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ArabazonPlatform.Domain.Domains.DMS.FileType> fileTypes =  _unitOfWork.GetRepository<ArabazonPlatform.Domain.Domains.DMS.FileType>().GetByExpression(p => request.FileTypeIds.Contains(p.FileTypeId)).ToList();
                    return new BaseResponse<List<ArabazonPlatform.Domain.Domains.DMS.FileType>>(true, string.Empty, fileTypes);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "FileTypeByIdsQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<ArabazonPlatform.Domain.Domains.DMS.FileType>>(false, ex.Message);
                }
            }
        }
    }
}
