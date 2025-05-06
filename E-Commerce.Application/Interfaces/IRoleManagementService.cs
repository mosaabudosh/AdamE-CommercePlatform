using E_Commerce.Application.Model;
using E_Commerce.Domain.Domains.Authorisation;
using Generic.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Interfaces
{
    public interface IRoleManagementService
    {
        Task<BaseResponse<Role>> AddRole(RoleModel roleModel);
        Task<BaseResponse> UpdateRole(RoleModel roleModel);
        Task<BaseResponse> DeleteRole(int roleId);
        Task<BaseResponse<List<RoleModel>>> GetRoles(int pageNumber, int pageSize);
        Task<BaseResponse<int>> GetRolesCount();
        Task<BaseResponse<RoleModel>> GetRoleById(int roleId);
    }
}
