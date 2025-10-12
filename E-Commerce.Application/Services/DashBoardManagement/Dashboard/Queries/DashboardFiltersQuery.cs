using Adam.Generic.Generic;
using Adam.Generic.Utility;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Store;
using E_Commerce.Application.StoredProcedure.Constants;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.DashBoardManagement.Dashboard.Queries
{
    public class DashboardFiltersQuery
    {
        public class Query : IRequest<BaseResponse<DashboardFiltersModel>>
        {
            public string? SearchText { get; set; }
            public string? Language { get; set; }
            public int? GenderId { get; set; }

            public Query(string? searchText = null,string? language = null, int? genderId = null)
            {
                SearchText = searchText;
                GenderId = genderId;
                Language = language;
            }
        }

        public class Handler(IConfiguration _configuration, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<DashboardFiltersModel>>
        {
            public async Task<BaseResponse<DashboardFiltersModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    DashboardFiltersModel dashboardFiltersModel = new DashboardFiltersModel() 
                    {
                        Brands = new List<DDLDataSource>(),
                        Categories = new List<DDLDataSource>(),
                        Colors = new List<DDLDataSource>(),
                        Discounts = new List<DDLDataSource>(),
                        Shops = new List<DDLDataSource>(),
                        Sizes = new List<DDLDataSource>()
                    };
                    IEnumerable<DDLDataSource> brandStoreFilters = null;
                    IEnumerable<DDLDataSource> categoryStoreFilter = null;
                    IEnumerable<DDLDataSource> shopStoreFilter = null;
                    IEnumerable<DDLDataSource> colorsStoreFilter = null;
                    IEnumerable<DDLDataSource> discountsStoreFilter = null;
                    IEnumerable<DDLDataSource> sizesStoreFilter = null;

                    using (DatabaseEngine databaseEngine = new DatabaseEngine(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        DapperParameters dapperParameters = new DapperParameters();
                        if (!string.IsNullOrEmpty(request.SearchText))
                        {
                            dapperParameters.AddParameter(ReportParameter.SearchText, request.SearchText);
                        }
                        if (!string.IsNullOrEmpty(request.Language))
                        {
                            dapperParameters.AddParameter(ReportParameter.Language, request.Language);
                        }
                        //if (request.GenderId.HasValue)
                        //{
                        //    dapperParameters.AddParameter("GenderId", request.GenderId.Value);
                        //}
                        brandStoreFilters = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.BrandStoreFilter, dapperParameters);
                        categoryStoreFilter = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.CategoryStoreFilter, dapperParameters);
                        shopStoreFilter = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.ShopStoreFilter, dapperParameters);
                        colorsStoreFilter = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.ColorsStoreFilter, dapperParameters);
                        discountsStoreFilter = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.DiscountsStoreFilter, dapperParameters);
                        sizesStoreFilter = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.SizesStoreFilter, dapperParameters);
                    }
                    if (brandStoreFilters is not null)
                    {
                        dashboardFiltersModel.Brands = brandStoreFilters.ToList();
                    }
                    if (categoryStoreFilter is not null)
                    {
                        dashboardFiltersModel.Categories = categoryStoreFilter.ToList();
                    }
                    if (shopStoreFilter is not null)
                    {
                        dashboardFiltersModel.Shops = shopStoreFilter.ToList();
                    }
                    if (colorsStoreFilter is not null)
                    {
                        dashboardFiltersModel.Colors = colorsStoreFilter.ToList();
                    }
                    if (discountsStoreFilter is not null)
                    {
                        dashboardFiltersModel.Discounts = discountsStoreFilter.ToList();
                    }
                    if (sizesStoreFilter is not null)
                    {
                        dashboardFiltersModel.Sizes = sizesStoreFilter.ToList();
                    }

                    return new BaseResponse<DashboardFiltersModel>(true, string.Empty, string.Empty, dashboardFiltersModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "DashboardFiltersQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<DashboardFiltersModel>(false, ex.Message);
                }
            }
        }
    }
}
