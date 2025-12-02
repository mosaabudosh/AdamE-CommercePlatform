using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.Authorisation;
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<BrandModel>>
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

                    List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSBrandPath, $"{_cachingPlatformSettings[PlatformSettingsCode.DMSBrandPath]}\\{brand.BrandId}"];
                    if (responseGetFileCreationModels.Any())
                    {
                        brand.FileCreationModel = responseGetFileCreationModels.FirstOrDefault();
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
