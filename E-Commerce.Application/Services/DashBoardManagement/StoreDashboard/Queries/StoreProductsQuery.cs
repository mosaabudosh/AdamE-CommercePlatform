using Adam.Generic.Utility;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Store;
using E_Commerce.Application.StoredProcedure.Constants;
using E_Commerce.Domain.IHttpServices;
using Generic.Response;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.DashBoardManagement.StoreDashboard.Queries
{
    public class StoreProductsQuery
    {
        public class Query : IRequest<BaseResponse<List<DashboardProductModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }
            public string? CategoryIds { get; set; }

            public Query(int pageNumber, int pageSize, string? searchText = null, string? categoryIds = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
                CategoryIds = categoryIds;
            }
        }

        public class Handler(IUserAccessor _userAccessor, IExceptionService _exceptionService, IConfiguration _configuration) : IRequestHandler<Query, BaseResponse<List<DashboardProductModel>>>
        {
            public async Task<BaseResponse<List<DashboardProductModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    IEnumerable<DashboardProductModel> dashboardProductModels;
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
                        dashboardProductModels = databaseEngine.ExecuteDapperSp<DashboardProductModel>(StoredProcedures.StoreProducts, dapperParameters);
                    }
                    if (dashboardProductModels is null)
                    {
                        return new BaseResponse<List<DashboardProductModel>>(false, "There is an implementation error on the database.");
                    }


                    return new BaseResponse<List<DashboardProductModel>>(true, string.Empty, string.Empty, dashboardProductModels.ToList());
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
