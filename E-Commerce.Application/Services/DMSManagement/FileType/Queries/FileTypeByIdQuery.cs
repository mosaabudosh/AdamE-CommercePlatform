using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.DMSManagement.FileType.Queries
{
    public class FileTypeByIdQuery
    {
        public class Query : IRequest<BaseResponse<E_Commerce.Domain.Domains.DMS.FileType>>
        {
            public int FileTypeId { get; set; }
            public Query(int fileTypeId)
            {
                this.FileTypeId = fileTypeId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<E_Commerce.Domain.Domains.DMS.FileType>>
        {
            public async Task<BaseResponse<E_Commerce.Domain.Domains.DMS.FileType>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    E_Commerce.Domain.Domains.DMS.FileType fileType = await _unitOfWork.GetRepository<E_Commerce.Domain.Domains.DMS.FileType>().GetById(request.FileTypeId);
                    return new BaseResponse<E_Commerce.Domain.Domains.DMS.FileType>(true, string.Empty, fileType);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "FileTypeByIdQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<E_Commerce.Domain.Domains.DMS.FileType>(false, ex.Message);
                }
            }
        }
    }
}
