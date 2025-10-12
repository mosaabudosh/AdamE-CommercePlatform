using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.ItemsManagement.Queries
{
    public class BrandQueryById
    {
        public class Query : IRequest<BaseResponse<BrandModel>>
        {
            public int BrandId { get; set; }
            public Query(int brandId)
            {
                BrandId = brandId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<BrandModel>>
        {
            public async Task<BaseResponse<BrandModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    BrandModel brand = _unitOfWork.GetRepository<Brand>().GetByExpressionAsNoTracking(p => p.BrandId == request.BrandId).Select(s => new BrandModel()
                    {
                        BrandId = s.BrandId,
                        IsViewInFooter = s.IsViewInFooter,
                        IsViewInHome = s.IsViewInHome,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Code = s.Code,
                        Descreption = s.Descreption,
                        Image = s.Image,
                        IsBlock = s.IsBlock,
                    }).FirstOrDefault();

                    string DMSBrandPath = _cachingPlatformSettings[PlatformSettingsCode.DMSBrandPath];
                    string folderPath = $"{DMSBrandPath}\\{brand.BrandId}";
                    BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFolderPathQuery.Query(folderPath));
                    if (!responseGetFileCreationModels.IsSuccess)
                    {
                        return new BaseResponse<BrandModel>(false, responseGetFileCreationModels.Message);
                    }
                    if (responseGetFileCreationModels.DataObject.Any())
                    {
                        brand.FileCreationModel = responseGetFileCreationModels.DataObject.FirstOrDefault();
                    }

                    return new BaseResponse<BrandModel>(true, string.Empty, string.Empty, brand);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "BrandQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<BrandModel>(false, ex.Message);
                }
            }
        }
    }
}
