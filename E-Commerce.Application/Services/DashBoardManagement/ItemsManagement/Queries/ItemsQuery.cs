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
    public class ItemsQuery
    {
        //public class Query : IRequest<BaseResponse<List<ItemModel>>>
        //{
        //    public int PageNumber { get; set; }
        //    public int PageSize { get; set; }
        //    public string? SearchText { get; set; }
        //    public int? CategoryId { get; set; }

        //    public Query(int pageNumber, int pageSize, string? searchText = null, int? categoryId = null)
        //    {
        //        PageNumber = pageNumber;
        //        PageSize = pageSize;
        //        SearchText = searchText;
        //        CategoryId = categoryId;
        //    }
        //}

        //public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<ItemModel>>>
        //{
        //    public async Task<BaseResponse<List<ItemModel>>> Handle(Query request, CancellationToken cancellationToken)
        //    {
        //        try
        //        {
        //            List<ItemModel> items = _unitOfWork.GetRepository<Item>().GetByExpressionAsNoTracking(p => (string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText) || p.Category.Name.Contains(request.SearchText) || p.Category.NameAr.Contains(request.SearchText)) && (!request.CategoryId.HasValue || p.CategoryId == request.CategoryId.Value)).Select(s => new ItemModel()
        //            {
        //                ItemId = s.ItemId,
        //                Name = s.Name,
        //                NameAr = s.NameAr,
        //                CategoryId = s.CategoryId,
        //                Category = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Category.Name : s.Category.NameAr,
        //                Code = s.Code,
        //                Descreption = s.Descreption,
        //                Image = s.Image,
        //                IsBlock = s.IsBlock,
        //                Images = new List<string>()
        //            }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

        //            if (items.Any(p => !string.IsNullOrEmpty(p.Image)))
        //            {
        //                string DMSItemPath = _cachingPlatformSettings[PlatformSettingsCode.DMSItemPath];

        //                List<string> filePaths = items.Where(p => !string.IsNullOrEmpty(p.Image)).Select(s => { return $"{DMSItemPath}\\{s.ItemId}"; }).ToList();
        //                BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
        //                if (!responseGetFileCreationModels.IsSuccess)
        //                {
        //                    return new BaseResponse<List<ItemModel>>(false, responseGetFileCreationModels.Message);
        //                }
        //                if (responseGetFileCreationModels.DataObject.Any())
        //                {
        //                    foreach (ItemModel item in items.Where(p => !string.IsNullOrEmpty(p.Image)))
        //                    {
        //                        item.Images.AddRange(responseGetFileCreationModels.DataObject.Where(p => p.FolderName == item.ItemId.ToString()).Select(s => s.FileContent));
        //                    }
        //                }
        //            }

        //            return new BaseResponse<List<ItemModel>>(true, string.Empty, string.Empty, items);
        //        }
        //        catch (Exception ex)
        //        {
        //            while (ex.InnerException is not null)
        //            {
        //                ex = ex.InnerException;
        //            }
        //            await _exceptionService.LogErrorAsync(ex, "ItemsQuery", _userAccessor.GetCurrentUserId(), request);
        //            return new BaseResponse<List<ItemModel>>(false, ex.Message);
        //        }
        //    }
        //}
    }
}
