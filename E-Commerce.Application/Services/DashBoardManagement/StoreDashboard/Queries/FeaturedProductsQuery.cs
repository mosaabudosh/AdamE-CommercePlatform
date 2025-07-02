using Adam.Generic.Utility;
using Adam.Logging.Interfaces;
using Adam.Logging.Services.MSSQLServer;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Model.Store;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Application.StoredProcedure.Constants;
using E_Commerce.Domain.Domains.Authorisation;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Commerce.Application.Services.DashBoardManagement.StoreDashboard.Queries
{
    public class FeaturedProductsQuery
    {
        public class Query : IRequest<BaseResponse<List<DashboardProductModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }

            public Query(int pageNumber, int pageSize)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
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

                        dashboardProductModels = databaseEngine.ExecuteDapperSp<DashboardProductModel>(StoredProcedures.FeaturedProducts, dapperParameters);
                    }
                    if(dashboardProductModels is null)
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
