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
    public class BrandsQuery
    {
        public class Query : IRequest<BaseResponse<List<BrandModel>>>
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<BrandModel>>>
        {
            public async Task<BaseResponse<List<BrandModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<BrandModel> brands = _unitOfWork.GetRepository<Brand>().GetByExpressionAsNoTracking(p => (string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText))).Select(s => new BrandModel()
                    {
                        BrandId = s.BrandId,
                        IsViewInHome = s.IsViewInHome,
                        IsViewInFooter = s.IsViewInFooter,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Code = s.Code,
                        Descreption = s.Descreption,
                        Image = s.Image,
                        IsBlock = s.IsBlock,
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (brands.Any(p => !string.IsNullOrEmpty(p.Image)))
                    {
                        string DMSBrandPath = _cachingPlatformSettings[PlatformSettingsCode.DMSBrandPath];

                        List<string> filePaths = brands.Where(p => !string.IsNullOrEmpty(p.Image)).Select(s => { return $"{DMSBrandPath}\\{s.BrandId}"; }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<BrandModel>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (BrandModel brand in brands.Where(p => !string.IsNullOrEmpty(p.Image)))
                            {
                                brand.Image = responseGetFileCreationModels.DataObject.FirstOrDefault(p => p.FolderName == brand.Image).FileContent;
                            }
                        }
                    }

                    return new BaseResponse<List<BrandModel>>(true, string.Empty, string.Empty, brands);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "BrandsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<BrandModel>>(false, ex.Message);
                }
            }
        }
    }
}
