

using ArabazonPlatform.Domain.Domains.Notification;

namespace ArabazonPlatform.Domain.Domains.Authorisation
{
	public class Role : SharedBase
	{
		public int RoleId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }

		public virtual List<UserRole> UserRoles { get; set; }
		public virtual List<RoleScreen> RoleScreens { get; set; }
        public virtual List<NotificationTypeRole> NotificationTypeRoles { get; set; }
    }
}
