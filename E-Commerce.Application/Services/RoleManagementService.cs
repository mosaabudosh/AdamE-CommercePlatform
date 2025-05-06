using Adam.Logging.Interfaces;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;
using Adam.Generic.Constants;
using System.Data;
using E_Commerce.Application.Interfaces;

namespace E_Commerce.Application.Services
{
    public class RoleManagementService : IRoleManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExceptionService _exceptionService;
        private readonly IUserAccessor _userAccessor;

        public RoleManagementService(IUnitOfWork unitOfWork,IExceptionService exceptionService, IUserAccessor userAccessor)
        {
            _unitOfWork = unitOfWork;
            _exceptionService = exceptionService;
            _userAccessor = userAccessor;
        }

        public async Task<BaseResponse<Role>> AddRole(RoleModel roleModel)
        {
            try
            {
                Role role = new Role()
                {
                    RoleId = 0,
                    Name = roleModel.Name.Trim(),
                    NameAr = !string.IsNullOrEmpty(roleModel.NameAr) ? roleModel.NameAr.Trim() : roleModel.Name.Trim(),
                    CreationDate = DateTime.UtcNow,
                    CreationUserId = _userAccessor.GetCurrentUserId(),
                    IsActive = true
                };
                BaseResponse validationBaseResponse = ValidateRole(role, false);
                if (!validationBaseResponse.IsSuccess)
                {
                    return new BaseResponse<Role>(false, string.Empty, ErrorCodes.Duplicate);
                }
                _unitOfWork.GetRepository<Role>().Insert(role);
                await _unitOfWork.CommitChanges();
                return new BaseResponse<Role>(true, string.Empty, role);

            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(AddRole), _userAccessor.GetCurrentUserId(), roleModel);
                return new BaseResponse<Role>(false, ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateRole(RoleModel roleModel)
        {
            try
            {
                Role role = new Role()
                {
                    RoleId = roleModel.RoleId,
                    Name = roleModel.Name.Trim(),
                    NameAr = !string.IsNullOrEmpty(roleModel.NameAr) ? roleModel.NameAr.Trim() : roleModel.Name.Trim(),
                    LastUpdatedDate = DateTime.UtcNow,
                    LastUpdateUserId = _userAccessor.GetCurrentUserId(),
                };

                BaseResponse validationBaseResponse = ValidateRole(role, true);
                if (!validationBaseResponse.IsSuccess)
                {
                    return new BaseResponse<Role>(false, string.Empty, ErrorCodes.Duplicate);
                }
                _unitOfWork.GetRepository<Role>().Update(role);
                await _unitOfWork.CommitChanges();
                return new BaseResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(UpdateRole), _userAccessor.GetCurrentUserId(), roleModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteRole(int roleId)
        {
            try
            {
                BaseResponse validationBaseResponse = ValidateDeletion(roleId);
                if (!validationBaseResponse.IsSuccess)
                {
                    return validationBaseResponse;
                }
                Role role = await _unitOfWork.GetRepository<Role>().GetById(roleId);
                _unitOfWork.GetRepository<Role>().Delete(role);
                await _unitOfWork.CommitChanges();
                return new BaseResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(DeleteRole), _userAccessor.GetCurrentUserId(), roleId);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<RoleModel>>> GetRoles(int pageNumber, int pageSize)
        {
            try
            {
                List<RoleModel> roles = _unitOfWork.GetRepository<Role>().GetByExpression().Select(s => new RoleModel()
                {
                    RoleId = s.RoleId,
                    Name = s.Name,
                    NameAr = s.NameAr,
                }).Skip(pageNumber * pageSize).Take(pageSize).ToList();

                return new BaseResponse<List<RoleModel>>(true, string.Empty, roles);
            }
            catch (Exception ex)
            {

                await _exceptionService.LogErrorAsync(ex, nameof(GetRoles), _userAccessor.GetCurrentUserId());
                return new BaseResponse<List<RoleModel>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<int>> GetRolesCount()
        {
            try
            {
                int rolesCount = _unitOfWork.GetRepository<Role>().GetByExpression().Count();
                return new BaseResponse<int>(true, string.Empty, string.Empty, rolesCount);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetRolesCount), _userAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<RoleModel>> GetRoleById(int roleId)
        {
            try
            {
                RoleModel role = _unitOfWork.GetRepository<Role>().GetByExpression(p => p.RoleId == roleId).Select(s => new RoleModel()
                {
                    RoleId = roleId,
                    Name = s.Name,
                    NameAr = s.NameAr,
                }).FirstOrDefault();

                return new BaseResponse<RoleModel>(true, string.Empty, role);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetRoleById), _userAccessor.GetCurrentUserId(), roleId);
                return new BaseResponse<RoleModel>(false, ex.Message);
            }
        }

        private BaseResponse ValidateRole(Role role, bool isUpdate)
        {
            try
            {
                Role existingRole = _unitOfWork.GetRepository<Role>().GetByExpression(x => x.Name == role.Name && (isUpdate ? (x.RoleId != role.RoleId) : true)).FirstOrDefault();

                if (existingRole != null)
                    return new BaseResponse(false, string.Empty, ErrorCodes.Exsist);

                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                _exceptionService.LogErrorAsync(ex, nameof(ValidateRole), _userAccessor.GetCurrentUserId(), role, isUpdate);
                return new BaseResponse<Role>(false, ex.Message);
            }
           
        }

        private BaseResponse ValidateDeletion(int roleId)
        {
            try
            {
                bool haveUserRoles = _unitOfWork.GetRepository<Role>().GetByExpression(i => i.RoleId == roleId && i.UserRoles.Any()).Any();
                if (haveUserRoles)
                {
                    return new BaseResponse(false, string.Empty, ErrorCodes.RelatedData);
                }
                return new BaseResponse(true, string.Empty);

            }
            catch (Exception ex)
            {
                _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), roleId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
