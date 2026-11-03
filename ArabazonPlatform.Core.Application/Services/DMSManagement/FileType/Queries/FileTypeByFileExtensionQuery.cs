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

namespace ArabazonPlatform.Application.Services.DMSManagement.FileType.Queries
{
    public class FileTypeByFileExtensionQuery
    {
        public class Query : IRequest<BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType>>
        {
            public string FileExtension { get; set; }
            public Query(string fileExtension)
            {
                this.FileExtension = fileExtension;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType>>
        {
            public async Task<BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    ArabazonPlatform.Domain.Domains.DMS.FileType fileType = _unitOfWork.GetRepository<ArabazonPlatform.Domain.Domains.DMS.FileType>().GetByExpressionAsNoTracking(x => request.FileExtension.Trim().ToLower() == x.Extension.ToLower()).FirstOrDefault();
                    return new BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType>(true, string.Empty, fileType);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "FileTypeByFileExtensionQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<ArabazonPlatform.Domain.Domains.DMS.FileType>(false, ex.Message);
                }
            }
        }
    }
}
