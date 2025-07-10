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
    public class FillDashboardProductData
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
                    List<string> imagesPaths = new List<string>();
                    BaseResponse<List<FileCreationModel>> responseImagesGetFileCreationModels = new BaseResponse<List<FileCreationModel>>();
                    List<string> shopeLogoPaths = new List<string>();
                    BaseResponse<List<FileCreationModel>> responseShopeLogoPathsGetFileCreationModels = new BaseResponse<List<FileCreationModel>>();

                    if (request.DashboardProductModels.Any(p => !string.IsNullOrEmpty(p.ImagesPaths)))
                    {
                        imagesPaths = request.DashboardProductModels.Where(p => !string.IsNullOrEmpty(p.ImagesPaths)).SelectMany(s =>
                        {
                            return s.ImagesPaths.Split(";").Select(i => { return i; });
                        }).ToList();
                       responseImagesGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(imagesPaths, false));
                        if (!responseImagesGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<DashboardProductModel>>(false, responseImagesGetFileCreationModels.Message);
                        }
                    }

                    if (request.DashboardProductModels.Any(p => !string.IsNullOrEmpty(p.ShopeLogoPath)))
                    {
                        shopeLogoPaths = request.DashboardProductModels.Where(p => !string.IsNullOrEmpty(p.ShopeLogoPath)).Select(s =>
                        {
                            return s.ShopeLogoPath;
                        }).ToList();
                        responseShopeLogoPathsGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(shopeLogoPaths, false));
                        if (!responseShopeLogoPathsGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<DashboardProductModel>>(false, responseShopeLogoPathsGetFileCreationModels.Message);
                        }                      
                    }

                    foreach (DashboardProductModel dashboardProductModel in request.DashboardProductModels.Where(p => !string.IsNullOrEmpty(p.ImagesPaths) || !string.IsNullOrEmpty(p.ShopeLogoPath) || (!string.IsNullOrEmpty(p.UnitNames) && !string.IsNullOrEmpty(p.UnitIds)) || (!string.IsNullOrEmpty(p.Colors) && !string.IsNullOrEmpty(p.ColorIds)) ))
                    {
                        dashboardProductModel.UnitModels = new List<DDLDataSource>();
                        dashboardProductModel.ColorModels = new List<DDLDataSource>();

                        if (imagesPaths.Any() && !string.IsNullOrEmpty(dashboardProductModel.ImagesPaths) && responseImagesGetFileCreationModels.DataObject.Any())
                        {
                            dashboardProductModel.Images = [];
                            dashboardProductModel.Images.AddRange(responseImagesGetFileCreationModels.DataObject.Where(p => p.FolderName == dashboardProductModel.ShopItemId.ToString()).Select(s => s.FileContent));
                            dashboardProductModel.Image = dashboardProductModel.Images[0];
                        }

                        if (shopeLogoPaths.Any() && !string.IsNullOrEmpty(dashboardProductModel.ShopeLogoPath) && responseShopeLogoPathsGetFileCreationModels.DataObject.Any())
                        {
                            dashboardProductModel.ShopeLogo = responseShopeLogoPathsGetFileCreationModels.DataObject.Where(p => p.FolderName == dashboardProductModel.ShopId.ToString()).Select(s => s.FileContent).FirstOrDefault();
                        }

                        if(!string.IsNullOrEmpty(dashboardProductModel.UnitNames) && !string.IsNullOrEmpty(dashboardProductModel.UnitIds))
                        {
                            List<string> unitNames = dashboardProductModel.UnitNames.Split(";").Select(i => { return i; }).ToList();
                            List<int> unitIds = dashboardProductModel.UnitIds.Split(";").Select(i => { return Convert.ToInt32(i); }).ToList();
                            for (int i = 0; i < unitIds.Count; i++)
                            {
                                dashboardProductModel.UnitModels.Add(new DDLDataSource()
                                {
                                    Label = unitNames[i],
                                    Value = unitIds[i],
                                });
                            }
                        }

                        if (!string.IsNullOrEmpty(dashboardProductModel.Colors) && !string.IsNullOrEmpty(dashboardProductModel.ColorIds))
                        {
                            List<string> colorNames = dashboardProductModel.Colors.Split(";").Select(i => { return i; }).ToList();
                            List<int> colorIds = dashboardProductModel.ColorIds.Split(";").Select(i => { return Convert.ToInt32(i); }).ToList();
                            for (int i = 0; i < colorIds.Count; i++)
                            {
                                dashboardProductModel.ColorModels.Add(new DDLDataSource()
                                {
                                    Label = colorNames[i],
                                    Value = colorIds[i],
                                });
                            }
                        }
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
