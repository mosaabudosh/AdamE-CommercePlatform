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
    public class ItemQueryById
    {
        //public class Query : IRequest<BaseResponse<ItemModel>>
        //{
        //    public Guid ItemId { get; set; }
        //    public Query(Guid itemId)
        //    {
        //        ItemId = itemId;
        //    }
        //}

        //public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<ItemModel>>
        //{
        //    public async Task<BaseResponse<ItemModel>> Handle(Query request, CancellationToken cancellationToken)
        //    {
        //        try
        //        {
        //            ItemModel item = _unitOfWork.GetRepository<Item>().GetByExpressionAsNoTracking(p => p.ItemId == request.ItemId).Select(s => new ItemModel()
        //            {
        //                ItemId = s.ItemId,
        //                CategoryId = s.CategoryId,
        //                Name = s.Name,
        //                NameAr = s.NameAr,
        //                Code = s.Code,
        //                Descreption = s.Descreption,
        //                Image = s.Image,
        //                IsBlock = s.IsBlock,
        //                Category = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Category.Name : s.Category.NameAr,
        //            }).FirstOrDefault();

        //            string DMSItemPath = _cachingPlatformSettings[PlatformSettingsCode.DMSItemPath];
        //            string folderPath = $"{DMSItemPath}\\{item.ItemId}";
        //            BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFolderPathQuery.Query(folderPath));
        //            if (!responseGetFileCreationModels.IsSuccess)
        //            {
        //                return new BaseResponse<ItemModel>(false, responseGetFileCreationModels.Message);
        //            }
        //            if (responseGetFileCreationModels.DataObject.Any())
        //            {
        //                item.FileCreationModels = responseGetFileCreationModels.DataObject;
        //            }

        //            return new BaseResponse<ItemModel>(true, string.Empty, string.Empty, item);
        //        }
        //        catch (Exception ex)
        //        {
        //            while (ex.InnerException is not null)
        //            {
        //                ex = ex.InnerException;
        //            }
        //            await _exceptionService.LogErrorAsync(ex, "ItemQueryById", _userAccessor.GetCurrentUserId());
        //            return new BaseResponse<ItemModel>(false, ex.Message);
        //        }
        //    }
        //}
    }
}
