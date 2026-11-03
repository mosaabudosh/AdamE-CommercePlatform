using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Generic.Utility;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.DashBoard;
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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.Dashboard.Queries
{
    public class DashboardFootersQuery
    {
        public class Query : IRequest<BaseResponse<DashboardFooterModel>>
        {
            public string? Language { get; set; }

            public Query(string? language = null)
            {
                Language = language;
            }
        }

        public class Handler(IConfiguration _configuration, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<DashboardFooterModel>>
        {
            public async Task<BaseResponse<DashboardFooterModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    DashboardFooterModel dashboardFooterModel = new DashboardFooterModel()
                    {
                        Beauty = new List<DDLDataSource>(),
                        Kids = new List<DDLDataSource>(),
                        MenFashion = new List<DDLDataSource>(),
                        TopBrands = new List<DDLDataSource>(),
                        WomenFashion = new List<DDLDataSource>()
                    };
                    IEnumerable<DDLDataSource> beautyFooter = null;
                    IEnumerable<DDLDataSource> kidsFooter = null;
                    IEnumerable<DDLDataSource> menFashionFooter = null;
                    IEnumerable<DDLDataSource> topBrandsFooter = null;
                    IEnumerable<DDLDataSource> womenFashionFooter = null;

                    using (DatabaseEngine databaseEngine = new DatabaseEngine(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        DapperParameters dapperParameters = new DapperParameters();
                        if (!string.IsNullOrEmpty(request.Language))
                        {
                            dapperParameters.AddParameter(ReportParameter.Language, request.Language);
                        }
                        beautyFooter = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.BeautyDashboardFooter, dapperParameters);
                        kidsFooter = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.KidsDashboardFooter, dapperParameters);
                        menFashionFooter = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.MenFashionsDashboardFooter, dapperParameters);
                        topBrandsFooter = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.TopBrandsDashboardFooter, dapperParameters);
                        womenFashionFooter = databaseEngine.ExecuteDapperSp<DDLDataSource>(StoredProcedures.WomenFashionsDashboardFooter, dapperParameters);
                    }
                    if (beautyFooter is not null)
                    {
                        dashboardFooterModel.Beauty = beautyFooter.ToList();
                    }
                    if (kidsFooter is not null)
                    {
                        dashboardFooterModel.Kids = kidsFooter.ToList();
                    }
                    if (menFashionFooter is not null)
                    {
                        dashboardFooterModel.MenFashion = menFashionFooter.ToList();
                    }
                    if (topBrandsFooter is not null)
                    {
                        dashboardFooterModel.TopBrands = topBrandsFooter.ToList();
                    }
                    if (womenFashionFooter is not null)
                    {
                        dashboardFooterModel.WomenFashion = womenFashionFooter.ToList();
                    }
                    return new BaseResponse<DashboardFooterModel>(true, string.Empty, string.Empty, dashboardFooterModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "DashboardFootersQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<DashboardFooterModel>(false, ex.Message);
                }
            }
        }
    }
}
