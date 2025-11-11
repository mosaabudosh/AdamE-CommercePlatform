using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.CMS;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Domain.Domains.CMS;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.CMSManagement.StaticPageManagement.Commands
{
    public class StaticPageUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public StaticPageModel StaticPageModel { get; set; }
            public Command(StaticPageModel staticPageModel)
            {
                StaticPageModel = staticPageModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    StaticPage exisitItem = await _unitOfWork.GetRepository<StaticPage>().GetById(request.StaticPageModel.StaticPageId);

                    exisitItem.Name = request.StaticPageModel.Name.Trim();
                    exisitItem.NameAr = request.StaticPageModel.NameAr.Trim();
                    exisitItem.Content = request.StaticPageModel.Content.Trim();
                    exisitItem.ContentAr = request.StaticPageModel.ContentAr.Trim();
                    exisitItem.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                    exisitItem.LastUpdatedDate = DateTime.UtcNow;

                    _unitOfWork.GetRepository<StaticPage>().Update(exisitItem);
                    await _unitOfWork.CommitChanges();

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "StaticPageUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
