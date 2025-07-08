using Adam.Logging.Interfaces;
using E_Commerce.Domain.Domains.DMS;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries
{
    public class FolderByPathQuery
    {
        public class Query : IRequest<BaseResponse<List<Folder>>>
        {
            public string FolderPath { get; set; }
            public Query(string folderPath)
            {
                FolderPath = folderPath;
            }
        }
        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<Folder>>>
        {
            public async Task<BaseResponse<List<Folder>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    IQueryable<Folder> foldersQuery = _unitOfWork.GetRepository<Folder>().GetByExpression(x => x.Path == request.FolderPath,includedEntities:new List<System.Linq.Expressions.Expression<Func<Folder, object>>> { z => z.Files});
                    List<Folder> folders = await _unitOfWork.GetRepository<Folder>().ToListAsync(foldersQuery);
                    return new BaseResponse<List<Folder>>(true, string.Empty, folders);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "FolderByPathCommands", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<Folder>>(false, ex.Message);
                }
            }

        }
    }
}
