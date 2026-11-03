using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.DMSManagement.FileType.Queries
{
    public class FileTypeByIdQuery
    {
        public class Query : IRequest<BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType>>
        {
            public int FileTypeId { get; set; }
            public Query(int fileTypeId)
            {
                this.FileTypeId = fileTypeId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType>>
        {
            public async Task<BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    ArabazonPlatform.Domain.Domains.DMS.FileType fileType = await _unitOfWork.GetRepository<ArabazonPlatform.Domain.Domains.DMS.FileType>().GetById(request.FileTypeId);
                    return new BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType>(true, string.Empty, fileType);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "FileTypeByIdQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType>(false, ex.Message);
                }
            }
        }
    }
}
