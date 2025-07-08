using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Store;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
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
    public class FilxDashboardProductData
    {
        public class Query : IRequest<BaseResponse<List<DashboardProductModel>>>
        {
            public List<DashboardProductModel> DashboardProductModels { get; set; }

            public Query(List<DashboardProductModel> dashboardProductModels)
            {
                DashboardProductModels = dashboardProductModels;
            }
        }

        public class Handler(IUserAccessor _userAccessor, IExceptionService _exceptionService,IMediator _mediator) : IRequestHandler<Query, BaseResponse<List<DashboardProductModel>>>
        {
            public async Task<BaseResponse<List<DashboardProductModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.DashboardProductModels.Any(p => !string.IsNullOrEmpty(p.ImagesPaths)))
                    {
                        List<string> filePaths = request.DashboardProductModels.Where(p => !string.IsNullOrEmpty(p.ImagesPaths)).SelectMany(s =>
                        {
                            return s.ImagesPaths.Split(";").Select(i => { return i; });
                        }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths, false));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<DashboardProductModel>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (DashboardProductModel dashboardProductModel in request.DashboardProductModels.Where(p => !string.IsNullOrEmpty(p.ImagesPaths)))
                            {
                                dashboardProductModel.Images = [];
                                dashboardProductModel.Images.AddRange(responseGetFileCreationModels.DataObject.Where(p => p.FolderName == dashboardProductModel.ShopItemId.ToString()).Select(s => s.FileContent));
                                dashboardProductModel.Image = dashboardProductModel.Images[0];
                            }
                        }
                    }

                    if (request.DashboardProductModels.Any(p => !string.IsNullOrEmpty(p.UnitNames) && !string.IsNullOrEmpty(p.UnitIds)))
                    {
                        request.DashboardProductModels.Where(p => !string.IsNullOrEmpty(p.UnitNames) && !string.IsNullOrEmpty(p.UnitIds)).Select(s =>
                        {
                            List<string> unitNames = s.UnitNames.Split(";").Select(i => { return i; }).ToList();
                            List<int> unitIds = s.UnitIds.Split(";").Select(i => { return Convert.ToInt32(i); }).ToList();
                            s.UnitModels = new List<DDLDataSource>();
                            for (int i = 0; i < unitIds.Count; i++)
                            {
                                s.UnitModels.Add(new DDLDataSource()
                                {
                                    Label = unitNames[i],
                                    Value = unitIds[i],
                                });
                            }
                            if (!s.SelectedUnitId.HasValue)
                            {
                                s.SelectedUnitId = s.UnitModels.FirstOrDefault().Value;
                            }
                            return s;
                        }).ToList();
                    }

                    return new BaseResponse<List<DashboardProductModel>>(true, string.Empty, string.Empty, request.DashboardProductModels);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "FilxDashboardProductData", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<DashboardProductModel>>(false, ex.Message);
                }
            }


        }
    }
}
