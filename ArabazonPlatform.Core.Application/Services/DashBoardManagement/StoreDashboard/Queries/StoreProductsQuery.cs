using ArabazonPlatform.Generic.Utility;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.Store;
using ArabazonPlatform.Application.StoredProcedure.Constants;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.StoreDashboard.Queries
{
    public class StoreProductsQuery
    {
        public class Query : IRequest<BaseResponse<List<DashboardProductModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }
            public string? CategoryIds { get; set; }
            public string? ShopIds { get; set; }
            public string? BrandIds { get; set; }
            public bool? IsLoadImage { get; set; }
            public decimal? FromPrice { get; set; }
            public decimal? ToPrice { get; set; }
            public bool? IsNewArrivals { get; set; }

            public Query(int pageNumber, int pageSize, string? searchText = null, string? categoryIds = null, string? shopIds = null,string? brandIds = null, bool? isLoadImage = true, bool? isNewArrivals = false, decimal? fromPrice = null, decimal? toPrice = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
                CategoryIds = categoryIds;
                ShopIds = shopIds;
                BrandIds = brandIds;
                IsLoadImage = isLoadImage;
                IsNewArrivals = isNewArrivals;
                FromPrice = fromPrice;
                ToPrice = toPrice;
            }
        }

        public class Handler(IUserAccessor _userAccessor, IExceptionService _exceptionService, IConfiguration _configuration, IMediator _mediator) : IRequestHandler<Query, BaseResponse<List<DashboardProductModel>>>
        {
            public async Task<BaseResponse<List<DashboardProductModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    IEnumerable<DashboardProductModel> dashboardProductModels = new List<DashboardProductModel>();
                    using (DatabaseEngine databaseEngine = new DatabaseEngine(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        DapperParameters dapperParameters = new DapperParameters();
                        dapperParameters.AddParameter(ReportParameter.PageNumber, request.PageNumber + 1);
                        dapperParameters.AddParameter(ReportParameter.PageSize, request.PageSize);
                        if (!string.IsNullOrEmpty(request.SearchText))
                        {
                            dapperParameters.AddParameter(ReportParameter.SearchText, request.SearchText);
                        }
                        if (!string.IsNullOrEmpty(request.CategoryIds))
                        {
                            dapperParameters.AddParameter("CategoryIds", request.CategoryIds);
                        }
                        if (!string.IsNullOrEmpty(request.ShopIds))
                        {
                            dapperParameters.AddParameter("ShopIds", request.ShopIds);
                        }
                        if (!string.IsNullOrEmpty(request.BrandIds))
                        {
                            dapperParameters.AddParameter("BrandIds", request.BrandIds);
                        }
                        if (request.IsLoadImage.HasValue && request.IsLoadImage.Value)
                        {
                            dapperParameters.AddParameter("IsLoadImage", request.IsLoadImage.Value);
                        }
                        if (request.IsLoadImage.HasValue && request.IsNewArrivals.Value)
                        {
                            dapperParameters.AddParameter("IsNewArrivals", request.IsNewArrivals.Value);
                        }
                        if (request.FromPrice.HasValue)
                        {
                            dapperParameters.AddParameter("FromPrice", request.FromPrice.Value);
                        }
                        if (request.ToPrice.HasValue)
                        {
                            dapperParameters.AddParameter("ToPrice", request.ToPrice.Value);
                        }
                        dashboardProductModels = databaseEngine.ExecuteDapperSp<DashboardProductModel>(StoredProcedures.StoreProducts, dapperParameters);
                    }
                    if (dashboardProductModels is null)
                    {
                        return new BaseResponse<List<DashboardProductModel>>(false, "There is an implementation error on the database.");
                    }
                    return await _mediator.Send(new FillDashboardProductData.Query(dashboardProductModels.ToList()));
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "FeaturedProductsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<DashboardProductModel>>(false, ex.Message);
                }
            }


        }
    }
}
