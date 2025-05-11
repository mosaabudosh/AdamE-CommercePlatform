

using E_Commerce.Application.Model;
using E_Commerce.Domain.Domains.Authorisation;
using Generic.Response;

namespace E_Commerce.Application.Interfaces
{
    public interface IUserManagementService
    {
        Task<BaseResponse> Registration(UserModel userModel);
        Task<BaseResponse<UserModel>> Login(string userName, string password);
        Task<BaseResponse> LogOut(string refreshToken);
        Task<BaseResponse<UserModel>> CheckRefreshAccessToken(string refreshToken);
        Task<BaseResponse> ChangePassword(Guid userId, string oldPassword, string newPassword);
        Task<UserLogin> GetUserLoginByUserId(Guid userId);
        Task<BaseResponse<List<UserModel>>> GetUsers(int pageNumber, int pageSize, string? searchText, int? cityId, int? shopId, int? languageId, bool? readProfileImage = true);
        Task<BaseResponse<int>> GetUsersCount(string? searchText, int? cityId, int? shopId, int? languageId);
        Task<BaseResponse<UserModel>> GetUserById(Guid userId);
        Task<BaseResponse> UpdateUser(UserModel userModel);
        Task<BaseResponse> DeleteUser(string userId);
    }
}
