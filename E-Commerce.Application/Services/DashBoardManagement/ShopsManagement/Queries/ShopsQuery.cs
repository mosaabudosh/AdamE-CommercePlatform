using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;

namespace E_Commerce.Application.Services.DashBoardManagement.ShopsManagement.Queries
{
    public class ShopsQuery
    {
        public class Query : IRequest<BaseResponse<List<ShopModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }
            public int? CityId { get; set; }
            public int? ParentShopId { get; set; }

            public string ShopPathDMS = "Shops";


            public Query(int pageNumber, int pageSize, string? searchText = null, int? cityId = null, int? parentShopId = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
                CityId = cityId;
                ParentShopId = parentShopId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Query, BaseResponse<List<ShopModel>>>
        {
            public async Task<BaseResponse<List<ShopModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ShopModel> shops = _unitOfWork.GetRepository<Shop>().GetByExpressionAsNoTracking(p => (string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.CommercialName.Contains(request.SearchText) || p.Descreption.Contains(request.SearchText) || p.About.Contains(request.SearchText)) && (!request.CityId.HasValue || p.CityId == request.CityId.Value) && (!request.ParentShopId.HasValue || p.ParentShopId == request.ParentShopId.Value)).Select(s => new ShopModel()
                    {
                        ShopId = s.ShopId,
                        ParentShopId = s.ParentShopId,
                        Logo = s.Logo,
                        Name = s.Name,
                        Code = s.Code,
                        Descreption = s.Descreption,
                        IsBlock = s.IsBlock,
                        About = s.About,
                        Adresse = s.Adresse,
                        CityId = s.CityId,
                        City = !s.CityId.HasValue ? null : _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.City.Name : s.City.NameAr,
                        CommercialName = s.CommercialName,
                        HasBranch = s.HasBranch,
                        NoOfBranch = s.NoOfBranch,
                        NoOfVendor = s.NoOfVendor,
                        PersonConcateName = s.PersonConcateName,
                        PhoneNumber = s.PhoneNumber
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (shops.Any(p => !string.IsNullOrEmpty(p.Logo)))
                    {
                        List<string> filePaths = shops.Where(p => !string.IsNullOrEmpty(p.Logo)).Select(s => { return $"{request.ShopPathDMS}\\{s.ShopId}"; }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<ShopModel>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (ShopModel shop in shops.Where(p => !string.IsNullOrEmpty(p.Logo)))
                            {
                                shop.Logo = responseGetFileCreationModels.DataObject.FirstOrDefault(p => p.FolderName == shop.ShopId.ToString())?.FileContent;
                            }
                        }
                    }

                    return new BaseResponse<List<ShopModel>>(true, string.Empty, string.Empty, shops);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<ShopModel>>(false, ex.Message);
                }
            }
        }
    }
}
