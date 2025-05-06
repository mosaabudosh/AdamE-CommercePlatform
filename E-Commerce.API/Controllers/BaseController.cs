using Adam.Logging.Interfaces;
using E_Commerce.Domain.IHttpServices;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        private IExceptionService _exceptionService;
        private IInformationService _informationService;
        private IUserAccessor _userAccessor;
        private IHttpAccessor _httpAccessor;

        protected IExceptionService ExceptionService => _exceptionService ?? (_exceptionService = HttpContext.RequestServices.GetService<IExceptionService>());
        protected IInformationService InformationService => _informationService ?? (_informationService = HttpContext.RequestServices.GetService<IInformationService>());
        protected IUserAccessor UserAccessor => _userAccessor ?? (_userAccessor = HttpContext.RequestServices.GetService<IUserAccessor>());
        protected IHttpAccessor HttpAccessor => _httpAccessor ?? (_httpAccessor = HttpContext.RequestServices.GetService<IHttpAccessor>());
    }
}
