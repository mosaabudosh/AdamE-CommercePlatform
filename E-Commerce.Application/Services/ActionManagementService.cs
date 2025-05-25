using Adam.Logging.Interfaces;
using E_Commerce.Application.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using static Adam.Generic.Constants.Constants;
using Action = E_Commerce.Domain.Domains.Authorisation.Action;

namespace E_Commerce.Application.Services
{
    public class ActionManagementService : IActionManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExceptionService _exceptionService;
        private readonly IUserAccessor _userAccessor;

        public ActionManagementService(IUnitOfWork unitOfWork, IExceptionService exceptionService, IUserAccessor userAccessor)
        {
            _unitOfWork = unitOfWork;
            _exceptionService = exceptionService;
            _userAccessor = userAccessor;
        }

        public async Task<BaseResponse<Domain.Domains.Authorisation.Action>> AddAction(ActionModel actionModel)
        {
            try
            {
                Domain.Domains.Authorisation.Action action = new Domain.Domains.Authorisation.Action()
                {
                    ActionId = 0,
                    Name = actionModel.Name.Trim(),
                    NameAr = !string.IsNullOrEmpty(actionModel.NameAr) ? actionModel.NameAr.Trim() : actionModel.Name.Trim(),
                    CreationDate = DateTime.UtcNow,
                    CreationUserId = _userAccessor.GetCurrentUserId(),
                    IsActive = true
                };
                BaseResponse validationBaseResponse = ValidateAction(action, false);
                if (!validationBaseResponse.IsSuccess)
                {
                    return new BaseResponse<Domain.Domains.Authorisation.Action>(false, string.Empty, ErrorCode.Duplicate);
                }
                _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().Insert(action);
                await _unitOfWork.CommitChanges();
                return new BaseResponse<Domain.Domains.Authorisation.Action>(true, string.Empty, action);

            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(AddAction), _userAccessor.GetCurrentUserId(), actionModel);
                return new BaseResponse<Domain.Domains.Authorisation.Action>(false, ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateAction(ActionModel actionModel)
        {
            try
            {
                Domain.Domains.Authorisation.Action action = new Domain.Domains.Authorisation.Action()
                {
                    ActionId = actionModel.ActionId,
                    Name = actionModel.Name.Trim(),
                    NameAr = !string.IsNullOrEmpty(actionModel.NameAr) ? actionModel.NameAr.Trim() : actionModel.Name.Trim(),
                    LastUpdatedDate = DateTime.UtcNow,
                    LastUpdateUserId = _userAccessor.GetCurrentUserId(),
                };

                BaseResponse validationBaseResponse = ValidateAction(action, true);
                if (!validationBaseResponse.IsSuccess)
                {
                    return new BaseResponse<Domain.Domains.Authorisation.Action>(false, string.Empty, ErrorCode.Duplicate);
                }
                _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().Update(action);
                await _unitOfWork.CommitChanges();
                return new BaseResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(UpdateAction), _userAccessor.GetCurrentUserId(), actionModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteAction(int actionId)
        {
            try
            {
                BaseResponse validationBaseResponse = ValidateDeletion(actionId);
                if (!validationBaseResponse.IsSuccess)
                {
                    return validationBaseResponse;
                }
                Domain.Domains.Authorisation.Action action = await _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetById(actionId);
                _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().Delete(action);
                await _unitOfWork.CommitChanges();
                return new BaseResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(DeleteAction), _userAccessor.GetCurrentUserId(), actionId);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<ActionModel>>> GetActions(int pageNumber, int pageSize)
        {
            try
            {
                List<ActionModel> actions = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpressionAsNoTracking().Select(s => new ActionModel()
                {
                    ActionId = s.ActionId,
                    Name = s.Name,
                    NameAr = s.NameAr,
                }).Skip(pageNumber * pageSize).Take(pageSize).ToList();

                return new BaseResponse<List<ActionModel>>(true, string.Empty, actions);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetActions), _userAccessor.GetCurrentUserId());
                return new BaseResponse<List<ActionModel>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<int>> GetActionsCount()
        {
            try
            {
                int actionsCount = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpression().Count();
                return new BaseResponse<int>(true, string.Empty, string.Empty, actionsCount);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetActionsCount), _userAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<ActionModel>> GetActionById(int actionId)
        {
            try
            {
                ActionModel action = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpressionAsNoTracking(p => p.ActionId == actionId).Select(s => new ActionModel()
                {
                    ActionId = actionId,
                    Name = s.Name,
                    NameAr = s.NameAr,
                }).FirstOrDefault();

                return new BaseResponse<ActionModel>(true, string.Empty, action);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetActionById), _userAccessor.GetCurrentUserId(), actionId);
                return new BaseResponse<ActionModel>(false, ex.Message);
            }
        }

        private BaseResponse ValidateAction(Domain.Domains.Authorisation.Action action, bool isUpdate)
        {
            try
            {
                Domain.Domains.Authorisation.Action existingAction = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpressionAsNoTracking(x => x.Name == action.Name && (isUpdate ? (x.ActionId != action.ActionId) : true)).FirstOrDefault();

                if (existingAction is not null)
                {
                    return new BaseResponse(false, string.Empty, ErrorCode.AlreadyExsist);
                }
                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                _exceptionService.LogErrorAsync(ex, nameof(ValidateAction), _userAccessor.GetCurrentUserId(), action, isUpdate);
                return new BaseResponse<Domain.Domains.Authorisation.Action>(false, ex.Message);
            }

        }

        private BaseResponse ValidateDeletion(int actionId)
        {
            try
            {
                bool haveUserActions = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpressionAsNoTracking(i => i.ActionId == actionId && i.RoleScreenActions.Any()).Any();
                if (haveUserActions)
                {
                    return new BaseResponse(false, string.Empty, ErrorCode.RelatedData);
                }
                return new BaseResponse(true, string.Empty);

            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), actionId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
