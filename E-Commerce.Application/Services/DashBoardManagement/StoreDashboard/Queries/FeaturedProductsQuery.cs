using Adam.Generic.Generic;
using Adam.Generic.Utility;
using Adam.Logging.Interfaces;
using Adam.Logging.Services.MSSQLServer;
using E_Commerce.Application.Caching;
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
using static E_Commerce.Domain.Enum.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Commerce.Application.Services.DashBoardManagement.StoreDashboard.Queries
{
    public class FeaturedProductsQuery
    {
        public class Query : IRequest<BaseResponse<List<DashboardProductModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public bool? IsLoadImage { get; set; }

            public Query(int pageNumber, int pageSize, bool? isLoadImage = true)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                IsLoadImage = isLoadImage;
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
                        if (request.IsLoadImage.HasValue && request.IsLoadImage.Value)
                        {
                            dapperParameters.AddParameter("IsLoadImage", request.IsLoadImage.Value);
                        }
                        dashboardProductModels = databaseEngine.ExecuteDapperSp<DashboardProductModel>(StoredProcedures.FeaturedProducts, dapperParameters);
                    }
                    if(dashboardProductModels is null)
                    {
                        return new BaseResponse<List<DashboardProductModel>>(false, "There is an implementation error on the database.");
                    }
                    return await _mediator.Send(new FilxDashboardProductData.Query(dashboardProductModels.ToList()));
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
