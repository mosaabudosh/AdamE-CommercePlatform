

using Adam.Generic.Generic;

namespace E_Commerce.Application.Model.Authorisation
{
    public class UserModel
    {
        public string? UserId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string? FirstNameAr { get; set; }
        public string? SecondNameAr { get; set; }
        public string? LastNameAr { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string? Password { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImage { get; set; }
        public string Adresse { get; set; }
        public bool? IsBlock { get; set; }
        public int UserTypeId { get; set; }
        public int LanguageId { get; set; }
        public int GenderId { get; set; }
        public string? Gender { get; set; }
        public string? Age { get; set; }
        public int CityId { get; set; }
        public int? ShopId { get; set; }
        public bool? IsActive { get; set; }
        public List<int>? RoleIds { get; set; }
        public List<UserMenu>? UserMenus { get; set; }
        public FileCreationModel? FileCreationModel { get; set; }
        public List<UserRoleModel>? UserRoles { get; set; }
        public List<UserSettingModel>? UserSettingModels { get; set; }

    }

    public class UserMenu
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string LabelAr { get; set; }
        public string Icon { get; set; }
        public bool Expanded { get; set; }
        public bool IsActive { get; set; }
        public List<UserMenuItem> UserMenuItems { get; set; }
    }

    public class UserMenuItem
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string LabelAr { get; set; }
        public string Icon { get; set; }
        public bool Expanded { get; set; }
        public bool IsActive { get; set; }
        public int ParentId { get; set; }
        public string URL { get; set; }
    }

    public class UserSettingModel
    {
        public int UserSettingId { get; set; }
        public int PlatformSettingId { get; set; }
        public Guid UserId { get; set; }
        public string? Value { get; set; }
    }


}
